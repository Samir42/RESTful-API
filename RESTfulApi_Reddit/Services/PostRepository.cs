using Microsoft.EntityFrameworkCore;
using RESTfulApi_Reddit.DbContexts;
using RESTfulApi_Reddit.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RESTfulApi_Reddit.Services {
    public class PostRepository : IPostRepository {

        private RedditDbContext _context;

        public PostRepository(RedditDbContext context) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<UserPost>> GetUserPosts(int userId) {
            return await _context.UserPosts.Include(x => x.User).Where(x => x.UserId == userId).ToListAsync();
        }

        public async Task<bool> SaveChangesAsync() {
            return (await _context.SaveChangesAsync() > 0);
        }

    }
}
