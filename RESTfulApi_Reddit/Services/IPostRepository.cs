using System.Collections.Generic;
using System.Threading.Tasks;
using RESTfulApi_Reddit.Entities;

namespace RESTfulApi_Reddit.Services {
    public interface IPostRepository {
        Task<IEnumerable<UserPost>> GetUserPosts(int userId);
        Task<bool> SaveChangesAsync();
    }
}