using Microsoft.EntityFrameworkCore;
using RESTfulApi_Reddit.DbContexts;
using RESTfulApi_Reddit.Entities;
using RESTfulApi_Reddit.Helpers;
using RESTfulApi_Reddit.ResourceParameter;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RESTfulApi_Reddit.Services
{
    public class UserRepository : IUserRepository
    {
        private RedditDbContext _context;

        public UserRepository(RedditDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<User> GetUserAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("userId can not be less than 1");
            }

            return await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        }

        public async Task<PagedList<User>> GetUsersAsync(ResourceParameters usersResourceParameters)
        {

            if (usersResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(usersResourceParameters));
            }

            var collection = _context.Users as IQueryable<User>;

            if (!string.IsNullOrWhiteSpace(usersResourceParameters.SearchQuery))
            {
                var searchQuery = usersResourceParameters.SearchQuery.Trim();

                collection = collection.Where(x => x.Name.Contains(searchQuery) || x.Surname.Contains(searchQuery)
                                                || x.About.Contains(searchQuery));
            }
            return await PagedList<User>.Create(collection,
                usersResourceParameters.PageNumber,
                usersResourceParameters.PageSize);
        }


        public void DeleteUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            _context.Users.Remove(user);
        }
        public async Task<bool> UserExistsAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("userId can not be less than 1");
            }

            return await _context.Users.AnyAsync(x => x.Id == userId);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() > 0);
        }
    }
}
