﻿using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
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
    [Route("api/users/{userId}/posts")]
    public class PostsController : BaseApiController
    {
        private readonly IPropertyCheckerService _propertyCheckerService;
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;

        public PostsController(IPostRepository postRepository,
            IPropertyCheckerService propertyCheckerService, IMapper mapper)
        {
            _postRepository = postRepository;
            _propertyCheckerService = propertyCheckerService;
            _mapper = mapper;
        }

        //We define inside Produces what we will return . This means we'll return app/json and app/hateoas+json
        [Produces("application/json",
            "application/vnd.marvin.hateoas+json")]
        [HttpGet("{userPostId}", Name = "GetUserPost")]
        public async Task<IActionResult> GetUserPost(int userPostId, string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {

            // we will not return another type 
            if (!MediaTypeHeaderValue.TryParse(mediaType,
                out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<UserPostDto>(fields))
            {
                return BadRequest();
            }

            var userPostFromRepo = await Mediator.Send(new GetUserPostByIdQuery(userPostId));

            if (userPostFromRepo == null)
            {
                return NotFound();
            }

            //Check whether parsedMediaType contains hateoas
            var includeLinks = parsedMediaType.SubTypeWithoutSuffix
               .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

            IEnumerable<LinkDto> links = new List<LinkDto>();


            var shapedUserPost = _mapper.Map<UserPostDto>(userPostFromRepo).ShapeData(fields) as IDictionary<string, object>;

            if (includeLinks)
            {
                links = CreateLinksForUserPost(userPostId, fields);
                shapedUserPost.Add("links", links);
            }

            return Ok(shapedUserPost);


            //var resourceToReturn = new {
            //    value=
            //}


            //we pass .hateoas to identify that what is accepted type. full or friendly

            //var primaryMediaType = includeLinks ?
            //   parsedMediaType.SubTypeWithoutSuffix
            //   .Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8)
            //   : parsedMediaType.SubTypeWithoutSuffix;

            //if(primaryMediaType == "vnd.marvin.userpos.full") {
            //    // i will return here userpost owner name and surname as Fulllname
            //    // else just send the userpost data

            //    var fullResourceToReturn = _mapper.Map<AuthorFullDto>(authorFromRepo)
            //            .ShapeData(fields) as IDictionary<string, object>;
            //}
        }

        [HttpDelete("{userPostId}", Name = "DeleteUserPost")]
        public async Task<IActionResult> DeleteUserPost(int userPostId)
        {
            await Mediator.Send(new DeleteUserPostCommand(userPostId));

            return NoContent();
        }

        [HttpPost(Name = "CreateUserPostForUser")]
        public async Task<IActionResult> CreateUserPost(int userId, UserPostForCreationDto userPostForCreationDto)
        {
            var userPostToReturn = await Mediator.Send(new CreateUserPostCommand(userId, userPostForCreationDto));

            return CreatedAtRoute("GetUserPostsForUser",
                new { userId = userId, userPostId = userPostToReturn.Id },
                userPostToReturn);
        }

        [HttpPatch("{userPostId}")]
        public async Task<IActionResult> PartiallyUpdateUserPostForUser(int userId, int userPostId,
            JsonPatchDocument<UserPostForUpdateDto> patchDocument)
        {
            bool userExists = await Mediator.Send(new UserExistsQuery(userId));

            if (!userExists)
            {
                return NotFound();
            }

            var userPostFromQuery = await Mediator.Send(new GetUserPostByUserAndPostIdQuery(userId, userPostId));

            //If does not exists create new one , add db
            if (userPostFromQuery == null)
            {
                var userPostDto = new UserPostForUpdateDto();

                //Add Validation
                patchDocument.ApplyTo(userPostDto, ModelState);

                if (!TryValidateModel(userPostDto))
                {
                    return ValidationProblem(ModelState);
                }

                var userPostToAdd = _mapper.Map<UserPost>(userPostDto);

                _postRepository.AddUserPost(userId, userPostToAdd);

                await _postRepository.SaveChangesAsync();

                var userPostToReturn = _mapper.Map<UserPostDto>(userPostToAdd);

                return CreatedAtRoute("GetUserPostsForUser",
                    new { userId = userId, userPostId = userPostToReturn.Id },
                    userPostToReturn);
            }

            var userPostToPatch = _mapper.Map<UserPostForUpdateDto>(userPostFromQuery);

            //Add validation
            patchDocument.ApplyTo(userPostToPatch, ModelState);

            if (!TryValidateModel(userPostToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(userPostToPatch, userPostFromQuery);

            _postRepository.UpdateUserPost(userPostFromQuery);

            await _postRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{userPostId}", Name = "UpdateUserPostForUser")]
        public async Task<IActionResult> UpdateUserPostForUser(int userId, int userPostId, UserPostForUpdateDto userPost)
        {
            bool userExists = await Mediator.Send(new UserExistsQuery(userId));

            if (!userExists)
            {
                return NotFound();
            }

            var userPostFromQuery = await Mediator.Send(new GetUserPostByUserAndPostIdQuery(userId, userPostId));

            //Create if userPost does not exists
            if (userPostFromQuery == null)
            {
                var userPostToAdd = _mapper.Map<UserPost>(userPost);
                //userPostToAdd.Id = userPostId;

                _postRepository.AddUserPost(userId, userPostToAdd);

                await _postRepository.SaveChangesAsync();

                var userPostToReturn = _mapper.Map<UserPostDto>(userPostToAdd);

                return CreatedAtRoute("GetUserPostsForUser",
                    new { userId = userId, userPostId = userPostToReturn.Id },
                    userPostToReturn);
            }

            //else update
            _mapper.Map(userPost, userPostFromQuery);

            _postRepository.UpdateUserPost(userPostFromQuery);

            await _postRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet(Name = "GetUserPostsForUser")]
        [HttpHead]
        public async Task<IActionResult> GetUserPosts([FromQuery] ResourceParameters postsResourceParameters)
        {

            if (!_propertyCheckerService.TypeHasProperties<UserPostDto>(postsResourceParameters.Fields))
            {
                return BadRequest();
            }

            var userPostsFromQuery = await Mediator.Send(new GetUserPostsQuery(postsResourceParameters));

            var paginationMetadata = new
            {
                totalCount = userPostsFromQuery.TotalCount,
                pageSize = userPostsFromQuery.PageSize,
                currentPage = userPostsFromQuery.CurrentPage,
                totalPages = userPostsFromQuery.TotalPages
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            var links = CreateLinksForUserPosts(postsResourceParameters,
                userPostsFromQuery.HasNext,
                userPostsFromQuery.HasPrevious);

            var shapedUserPosts = _mapper.Map<IEnumerable<UserPostDto>>(userPostsFromQuery).ShapeData(postsResourceParameters.Fields);

            var shapedUserPostsWithLinks = shapedUserPosts.Select(userPost =>
            {
                var userPostAsDictionary = userPost as IDictionary<string, object>;
                var userPostLinks = CreateLinksForUserPost((int)userPostAsDictionary["Id"], null);
                userPostAsDictionary.Add("links", userPostLinks);
                return userPostAsDictionary;
            });


            // To be cleaner and understandeable
            var linkedCollectionResource = new
            {
                value = shapedUserPostsWithLinks,
                links
            };

            return Ok(linkedCollectionResource);
        }


        private IEnumerable<LinkDto> CreateLinksForUserPost(int userPostId, string fields)
        {
            var links = new List<LinkDto>();


            //If data shaping haven't used    return simple Link , else return shaped data link
            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new LinkDto(Url.Link("GetUserPost", new { userPostId }),
                    "self",
                    "GET"));
            }
            else
            {
                links.Add(new LinkDto(Url.Link("GetUserPost", new { userPostId, fields }),
                    "self",
                    "GET"));
            }


            links.Add(
              new LinkDto(Url.Link("DeleteUserPost", new { userPostId }),
              "delete_userpost_for_user",
              "DELETE"));

            links.Add(
                new LinkDto(Url.Link("CreateUserPostForUser", new { userPostId }),
                "create_userpost_for_user",
                "POST"));

            //links.Add(
            //   new LinkDto(Url.Link("GetUserPostsForUser", new { userId }),
            //   "userposts",
            //   "GET"));

            return links;
        }


        private IEnumerable<LinkDto> CreateLinksForUserPosts(ResourceParameters resourceParameters,
            bool hasNext, bool hasPrevious)
        {

            var links = new List<LinkDto>();

            links.Add(new LinkDto(CreateUserPostsResourceUri(resourceParameters, ResourceUriType.CurrentPage),
                "self", "GET"));

            if (hasNext)
            {
                links.Add(new LinkDto(CreateUserPostsResourceUri(resourceParameters, ResourceUriType.NextPage),
               "self", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(new LinkDto(CreateUserPostsResourceUri(resourceParameters, ResourceUriType.PreviousPage),
               "self", "GET"));
            }

            return links;

        }


        private string CreateUserPostsResourceUri(ResourceParameters resourceParameters,
            ResourceUriType type)
        {


            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetUserPostsForUser",
                        new
                        {
                            fields = resourceParameters.Fields,
                            pageNumber = resourceParameters.PageNumber - 1,
                            pageSize = resourceParameters.PageSize,
                            searchQuery = resourceParameters.SearchQuery
                        });
                case ResourceUriType.NextPage:
                    return Url.Link("GetUserPostsForUser",
                        new
                        {
                            fields = resourceParameters.Fields,
                            pageNumber = resourceParameters.PageNumber + 1,
                            pageSize = resourceParameters.PageSize,
                            searchQuery = resourceParameters.SearchQuery
                        });
                // Do nothing here. Because [default] will do what it takes.
                case ResourceUriType.CurrentPage:
                default:
                    return Url.Link("GetUserPostsForUser",
                       new
                       {
                           fields = resourceParameters.Fields,
                           pageNumber = resourceParameters.PageNumber,
                           pageSize = resourceParameters.PageSize,
                           searchQuery = resourceParameters.SearchQuery
                       });

            }
        }
    }
}
