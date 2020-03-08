using System.Threading.Tasks;

namespace RESTfulApi_Reddit.Services {
    public interface IUserRepository {
        Task<bool> UserExistsAsync(int userId);
    }
}