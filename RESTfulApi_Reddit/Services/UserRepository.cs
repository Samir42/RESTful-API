using Microsoft.EntityFrameworkCore;
using RESTfulApi_Reddit.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RESTfulApi_Reddit.Services {
    public class UserRepository : IUserRepository {
        private RedditDbContext _context;

        public UserRepository(RedditDbContext context) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> UserExistsAsync(int userId) {
            if (userId <= 0) {
                throw new ArgumentException("userId can not be less than 1");
            }

            return await _context.Users.AnyAsync(x => x.Id == userId);
        }
    }
}
