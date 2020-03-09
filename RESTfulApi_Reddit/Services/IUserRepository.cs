using RESTfulApi_Reddit.Entities;
using RESTfulApi_Reddit.Helpers;
using RESTfulApi_Reddit.ResourceParameter;
using System.Threading.Tasks;

namespace RESTfulApi_Reddit.Services {
    public interface IUserRepository {
        Task<User> GetUserAsync(int userId);
        Task<PagedList<User>> GetUsersAsync(ResourceParameters usersResourceParameters);
        Task<bool> UserExistsAsync(int userId);
    }
}