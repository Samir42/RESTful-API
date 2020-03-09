using RESTfulApi_Reddit.Entities;
using System.Threading.Tasks;

namespace RESTfulApi_Reddit.Services {
    public interface IUserRepository {
        Task<User> GetUserAsync(int userId);
        Task<bool> UserExistsAsync(int userId);
    }
}