using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using RESTfulApi_Reddit.Entities;
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
        private readonly IUserRepository _userRepository;

        public PostsController(IPostRepository postRepository, IUserRepository userRepository,
            IPropertyCheckerService propertyCheckerService, IMapper mapper) {
            _postRepository = postRepository ??
                throw new ArgumentNullException(nameof(postRepository));
            _propertyCheckerService = propertyCheckerService ??
                throw new ArgumentNullException(nameof(propertyCheckerService));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _userRepository = userRepository ??
               throw new ArgumentNullException(nameof(userRepository));
        }

        //We define inside Produces what we will return . This means we'll return app/json and app/hateoas+json
        [Produces("application/json",
            "application/vnd.marvin.hateoas+json")]
        [HttpGet("{userPostId}", Name = "GetUserPost")]
        public async Task<IActionResult> GetUserPost(int userPostId, string fields,
            [FromHeader(Name = "Accept")]string mediaType) {

            // we will not return another type 
            if (!MediaTypeHeaderValue.TryParse(mediaType,
                out MediaTypeHeaderValue parsedMediaType)) {
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<UserPostDto>(fields)) {
                return BadRequest();
            }

            var userPostFromRepo = await _postRepository.GetUserPostAsync(userPostId);

            if (userPostFromRepo == null) {
                return NotFound();
            }

            //Check whether parsedMediaType contains hateoas
            var includeLinks = parsedMediaType.SubTypeWithoutSuffix
               .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

            IEnumerable<LinkDto> links = new List<LinkDto>();


            var shapedUserPost = _mapper.Map<UserPostDto>(userPostFromRepo).ShapeData(fields) as IDictionary<string,object>;

            if (includeLinks) {
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

        [HttpDelete("{userPostId}",Name ="DeleteUserPost")]
        public async Task<IActionResult> DeleteUserPost(int userPostId) {
            var userPost = await _postRepository.GetUserPostAsync(userPostId);

            if (userPost == null) {
                return NotFound();
            }

            _postRepository.DeleteUserPost(userPost);

            await _postRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost(Name ="CreateUserPostForUser")]
        public async Task<IActionResult> CreateUserPost(int userId,UserPostForCreationDto userPostForCreationDto) {
            if (! await _userRepository.UserExistsAsync(userId)) {
                return NotFound();
            }

            var userPostEntity = _mapper.Map<UserPost>(userPostForCreationDto);

            _postRepository.AddUserPost(userId, userPostEntity);

            await _postRepository.SaveChangesAsync();

            var userPostToReturn = _mapper.Map<UserPostDto>(userPostEntity);

            return CreatedAtRoute("GetUserPostsForUser",
                new { userId = userId, userPostId = userPostToReturn.Id },
                userPostToReturn);
        }

        [HttpGet(Name = "GetUserPostsForUser")]
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

            var links = CreateLinksForUserPosts(postsResourceParameters,
                userPostsFromRepo.HasNext,
                userPostsFromRepo.HasPrevious);

            var shapedUserPosts = _mapper.Map<IEnumerable<UserPostDto>>(userPostsFromRepo).ShapeData(postsResourceParameters.Fields);

            var shapedUserPostsWithLinks = shapedUserPosts.Select(userPost => {
                var userPostAsDictionary = userPost as IDictionary<string, object>;
                var userPostLinks = CreateLinksForUserPost((int)userPostAsDictionary["Id"], null);
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


        private IEnumerable<LinkDto> CreateLinksForUserPost(int userPostId, string fields) {
            var links = new List<LinkDto>();


            //If data shaping haven't used    return simple Link , else return shaped data link
            if (string.IsNullOrWhiteSpace(fields)) {
                links.Add(new LinkDto(Url.Link("GetUserPost", new { userPostId }),
                    "self",
                    "GET"));
            }
            else {
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


        private IEnumerable<LinkDto> CreateLinksForUserPosts(PostsResourceParameters postsResourceParameters,
            bool hasNext, bool hasPrevious) {

            var links = new List<LinkDto>();

            links.Add(new LinkDto(CreateUserPostsResourceUri(postsResourceParameters, ResourceUriType.CurrentPage),
                "self", "GET"));

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
                            pageNumber = postsResourceParameters.PageNumber - 1,
                            pageSize = postsResourceParameters.PageSize,
                            searchQuery = postsResourceParameters.SearchQuery
                        });
                case ResourceUriType.NextPage:
                    return Url.Link("GetUserPostsForUser",
                        new {
                            fields = postsResourceParameters.Fields,
                            pageNumber = postsResourceParameters.PageNumber + 1,
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
