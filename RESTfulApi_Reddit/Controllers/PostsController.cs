using Microsoft.AspNetCore.Mvc;
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

        public PostsController(IPostRepository postRepository) {
            _postRepository = postRepository ??
                throw new ArgumentNullException(nameof(postRepository));
        }

        [HttpGet]
        public async Task<IActionResult> GetUserPosts(int userId) {
            var postEntites = await _postRepository.GetUserPosts(userId);

            return Ok(postEntites);
        }
    }
}
