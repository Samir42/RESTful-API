using AutoMapper;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using RESTfulApi_Reddit.AppServices.Commands;
using RESTfulApi_Reddit.AppServices.Queries;
using RESTfulApi_Reddit.Entities;
using RESTfulApi_Reddit.Helpers;
using RESTfulApi_Reddit.Models;
using RESTfulApi_Reddit.ResourceParameter;
using RESTfulApi_Reddit.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace RESTfulApi_Reddit.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : BaseApiController
    {
        private readonly IPropertyCheckerService _propertyCheckerService;
        private readonly IMapper _mapper;

        public UsersController(IPropertyCheckerService propertyCheckerService, IMapper mapper)
        {
            _propertyCheckerService = propertyCheckerService;
            _mapper = mapper;
        }

        [Produces("application/json",
            "application/vnd.marvin.hateoas+json",
            "application/vnd.marvin.user.full+json",
            "application/vnd.marvin.user.full.hateoas+json",
            "application/vnd.marvin.user.friendly+json",
            "application/vnd.marvin.user.friendly.hateoas+json")]
        [HttpGet("{userId}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int userId, string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType,
                out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<User>(fields))
            {
                return BadRequest();
            }

            var serFromQueryHandler = await Mediator.Send(new GetUserQuery(userId));

            if (serFromQueryHandler == null)
            {
                return NotFound();
            }



            //Check whether links wanted
            var includeLinks = parsedMediaType.SubTypeWithoutSuffix
               .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

            IEnumerable<LinkDto> links = new List<LinkDto>();

            if (includeLinks)
            {
                links = CreateLinksForUser(userId, fields);
            }


            //we pass .hateoas to identify that what is accepted type. full or friendly
            var primaryMediaType = includeLinks ?
               parsedMediaType.SubTypeWithoutSuffix
               .Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8)
               : parsedMediaType.SubTypeWithoutSuffix;

            if (primaryMediaType == "vnd.marvin.user.full")
            {
                // i will return here userpost owner name and surname as Fulllname
                // else just send the userpost data

                var fullResourceToReturn = _mapper.Map<UserFullDto>(serFromQueryHandler)
                        .ShapeData(fields) as IDictionary<string, object>;

                if (includeLinks)
                {
                    fullResourceToReturn.Add("links", links);
                }

                return Ok(fullResourceToReturn);
            }

            var friendlyResourceToReturn = _mapper.Map<UserDto>(serFromQueryHandler)
                        .ShapeData(fields) as IDictionary<string, object>;

            if (includeLinks)
            {
                friendlyResourceToReturn.Add("links", links);
            }

            return Ok(friendlyResourceToReturn);
        }

        [HttpGet(Name = "GetUsers")]
        [HttpHead]
        public async Task<IActionResult> GetUsers(
            [FromQuery] ResourceParameters userResourceParameters)
        {
            if (!_propertyCheckerService.TypeHasProperties<UserDto>(userResourceParameters.Fields))
            {
                return BadRequest();
            }

            var usersFromQueryHandler = await Mediator.Send(new GetListQuery(userResourceParameters));

            var paginationMetadata = new
            {
                totalCount = usersFromQueryHandler.TotalCount,
                pageSize = usersFromQueryHandler.PageSize,
                currentPage = usersFromQueryHandler.CurrentPage,
                totalPages = usersFromQueryHandler.TotalPages
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            var links = CreateLinksForUsers(userResourceParameters,
                usersFromQueryHandler.HasNext,
                usersFromQueryHandler.HasPrevious);

            var shapedUsers = _mapper.Map<IEnumerable<UserDto>>(usersFromQueryHandler)
                               .ShapeData(userResourceParameters.Fields);

            var shapedUsersWithLinks = shapedUsers.Select(user =>
            {
                var userAsDictionary = user as IDictionary<string, object>;
                var userLinks = CreateLinksForUser((int)userAsDictionary["Id"], null);
                userAsDictionary.Add("links", userLinks);
                return userAsDictionary;
            });

            var linkedCollectionResource = new
            {
                value = shapedUsersWithLinks,
                links
            };

            return Ok(linkedCollectionResource);
        }

        [HttpDelete("{userId}", Name = "DeleteUser")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var result = await Mediator.Send(new DeleteUserCommand(userId));

            if (result == 0)
                return NotFound();
            else
                return Ok();
        }

        private IEnumerable<LinkDto> CreateLinksForUser(int userId, string fields)
        {
            var links = new List<LinkDto>();


            //If data shaping haven't used    return simple Link , else return shaped data link
            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new LinkDto(Url.Link("GetUser", new { userId }),
                    "self",
                    "GET"));
            }
            else
            {
                links.Add(new LinkDto(Url.Link("GetUser", new { userId, fields }),
                    "self",
                    "GET"));
            }


            links.Add(
              new LinkDto(Url.Link("DeleteUser", new { userId }),
              "delete_user",
              "DELETE"));

            links.Add(
                new LinkDto(Url.Link("CreateUser", new { userId }),
                "create_user",
                "POST"));

            //links.Add(
            //   new LinkDto(Url.Link("GetUserPostsForUser", new { userId }),
            //   "userposts",
            //   "GET"));

            return links;
        }


        private IEnumerable<LinkDto> CreateLinksForUsers(ResourceParameters resourceParameters,
            bool hasNext, bool hasPrevious)
        {

            var links = new List<LinkDto>();

            links.Add(new LinkDto(CreateUsersResourceUri(resourceParameters, ResourceUriType.CurrentPage),
                "self", "GET"));

            if (hasNext)
            {
                links.Add(new LinkDto(CreateUsersResourceUri(resourceParameters, ResourceUriType.NextPage),
               "self", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(new LinkDto(CreateUsersResourceUri(resourceParameters, ResourceUriType.PreviousPage),
               "self", "GET"));
            }

            return links;

        }


        private string CreateUsersResourceUri(ResourceParameters usersResourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetUser",
                        new
                        {
                            fields = usersResourceParameters.Fields,
                            pageNumber = usersResourceParameters.PageNumber - 1,
                            pageSize = usersResourceParameters.PageSize,
                            searchQuery = usersResourceParameters.SearchQuery
                        });
                case ResourceUriType.NextPage:
                    return Url.Link("GetUser",
                        new
                        {
                            fields = usersResourceParameters.Fields,
                            pageNumber = usersResourceParameters.PageNumber + 1,
                            pageSize = usersResourceParameters.PageSize,
                            searchQuery = usersResourceParameters.SearchQuery
                        });
                // Do nothing here. Because [default] will do what it takes.
                case ResourceUriType.CurrentPage:
                default:
                    return Url.Link("GetUser",
                       new
                       {
                           fields = usersResourceParameters.Fields,
                           pageNumber = usersResourceParameters.PageNumber,
                           pageSize = usersResourceParameters.PageSize,
                           searchQuery = usersResourceParameters.SearchQuery
                       });

            }
        }
    }
}
