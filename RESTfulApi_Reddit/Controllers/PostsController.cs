using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RESTfulApi_Reddit.Helpers;
using RESTfulApi_Reddit.Models;
using RESTfulApi_Reddit.ResourceParameters;
using RESTfulApi_Reddit.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace RESTfulApi_Reddit.Controllers {
    [ApiController]
    [Route("api/users/{userId}/posts")]
    public class PostsController : ControllerBase {
        private readonly IPostRepository _postRepository;
        private readonly IPropertyCheckerService _propertyCheckerService;
        private readonly IMapper _mapper;

        public PostsController(IPostRepository postRepository, IPropertyCheckerService propertyCheckerService,IMapper mapper) {
            _postRepository = postRepository ??
                throw new ArgumentNullException(nameof(postRepository));
            _propertyCheckerService = propertyCheckerService ??
                throw new ArgumentNullException(nameof(propertyCheckerService));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet(Name ="GetUserPostsForUser")]
        [HttpHead]
        public async Task<IActionResult> GetUserPosts([FromQuery]PostsResourceParameters postsResourceParameters) {

            if (!_propertyCheckerService.TypeHasProperties<UserPostDto>(postsResourceParameters.Fields)) {
                return BadRequest();
            }

            var userPostsFromRepo = await _postRepository.GetUserPostsAsync(postsResourceParameters);

            var paginationMetadata = new {
                totalCount = userPostsFromRepo.TotalCount,
                pageSize = userPostsFromRepo.PageSize,
                currentPage = userPostsFromRepo.CurrentPage,
                totalPages = userPostsFromRepo.TotalPages
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            var links = CreateLinksForAuthors(postsResourceParameters,
                userPostsFromRepo.HasNext,
                userPostsFromRepo.HasPrevious);

            var shapedUserPosts = _mapper.Map<IEnumerable<UserPostDto>>(userPostsFromRepo).ShapeData(postsResourceParameters.Fields);

            var shapedUserPostsWithLinks = shapedUserPosts.Select(userPost => {
                var userPostAsDictionary = userPost as IDictionary<string, object>;
                var userPostLinks = CreateLinksForAuthor((int)userPostAsDictionary["Id"], null);
                userPostAsDictionary.Add("links", userPostLinks);
                return userPostAsDictionary;
            });


            // To be cleaner and understandeable
            var linkedCollectionResource = new {
                value = shapedUserPostsWithLinks,
                links
            };

            return Ok(linkedCollectionResource);
        }


        private IEnumerable<LinkDto> CreateLinksForAuthor(int userId,string fields) {
            var links = new List<LinkDto>();


            //If data shaping haven't used    return simple Link , else return shaped data link
            if (string.IsNullOrWhiteSpace(fields)) {
                links.Add(new LinkDto(Url.Link("GetAuthor", new { userId }),
                    "self",
                    "GET"));
            }
            else {
                links.Add(new LinkDto(Url.Link("GetAuthor", new { userId, fields }),
                    "self",
                    "GET"));
            }


            links.Add(
              new LinkDto(Url.Link("DeleteUserPost", new { userId }),
              "delete_userpost_for_user",
              "DELETE"));

            links.Add(
                new LinkDto(Url.Link("CreateUserPostForUser", new { userId }),
                "create_userpost_for_user",
                "POST"));

            links.Add(
               new LinkDto(Url.Link("GetUserPostsForUser", new { userId }),
               "userposts",
               "GET"));

            return links;
        }


        private IEnumerable<LinkDto> CreateLinksForAuthors(PostsResourceParameters postsResourceParameters,
            bool hasNext,bool hasPrevious) {

            var links = new List<LinkDto>();

            links.Add(new LinkDto(CreateUserPostsResourceUri(postsResourceParameters,ResourceUriType.CurrentPage),
                "self","GET"));

            if (hasNext) {
                links.Add(new LinkDto(CreateUserPostsResourceUri(postsResourceParameters, ResourceUriType.NextPage),
               "self", "GET"));
            }

            if (hasPrevious) {
                links.Add(new LinkDto(CreateUserPostsResourceUri(postsResourceParameters, ResourceUriType.PreviousPage),
               "self", "GET"));
            }

            return links;

        }


        private string CreateUserPostsResourceUri(PostsResourceParameters postsResourceParameters,
            ResourceUriType type) {


            switch (type) {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetUserPostsForUser",
                        new {
                            fields = postsResourceParameters.Fields,
                            pageNumber = postsResourceParameters.PageNumber-1,
                            pageSize = postsResourceParameters.PageSize,
                            searchQuery = postsResourceParameters.SearchQuery
                        });
                case ResourceUriType.NextPage:
                    return Url.Link("GetUserPostsForUser",
                        new {
                            fields = postsResourceParameters.Fields,
                            pageNumber = postsResourceParameters.PageNumber+1,
                            pageSize = postsResourceParameters.PageSize,
                            searchQuery = postsResourceParameters.SearchQuery
                        });
                // Do nothing here. Because [default] will do what it takes.
                case ResourceUriType.CurrentPage:
                default:
                    return Url.Link("GetUserPostsForUser",
                       new {
                           fields = postsResourceParameters.Fields,
                           pageNumber = postsResourceParameters.PageNumber,
                           pageSize = postsResourceParameters.PageSize,
                           searchQuery = postsResourceParameters.SearchQuery
                       });

            }
        }
    }
}
