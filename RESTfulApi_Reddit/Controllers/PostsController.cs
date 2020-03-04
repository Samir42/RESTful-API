using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RESTfulApi_Reddit.Helpers;
using RESTfulApi_Reddit.Models;
using RESTfulApi_Reddit.ResourceParameters;
using RESTfulApi_Reddit.Services;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [HttpGet]
        [HttpHead]
        public async Task<IActionResult> GetUserPosts([FromQuery]PostsResourceParameters postsResourceParameters) {

            if (!_propertyCheckerService.TypeHasProperties<UserPostDto>(postsResourceParameters.Fields)) {
                return BadRequest();
            }


            var userPostsFromRepo = await _postRepository.GetUserPostsAsync(postsResourceParameters);

            var shapedUserPosts = _mapper.Map<IEnumerable<UserPostDto>>(userPostsFromRepo).ShapeData(postsResourceParameters.Fields);


            return Ok(shapedUserPosts);
        }
    }
}
