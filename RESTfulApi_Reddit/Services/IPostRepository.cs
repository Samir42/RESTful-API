using System.Collections.Generic;
using System.Threading.Tasks;
using RESTfulApi_Reddit.Entities;
using RESTfulApi_Reddit.Helpers;
using RESTfulApi_Reddit.ResourceParameters;

namespace RESTfulApi_Reddit.Services {
    public interface IPostRepository {
        Task<PagedList<UserPost>> GetUserPostsAsync(PostsResourceParameters postsResourceParameters);
        Task<bool> SaveChangesAsync();
    }
}