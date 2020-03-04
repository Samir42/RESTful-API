using Microsoft.EntityFrameworkCore;
using RESTfulApi_Reddit.DbContexts;
using RESTfulApi_Reddit.Entities;
using RESTfulApi_Reddit.Helpers;
using RESTfulApi_Reddit.ResourceParameters;
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

        public async Task<PagedList<UserPost>> GetUserPostsAsync(PostsResourceParameters postsResourceParameters) {

            if (postsResourceParameters == null) {
                throw new ArgumentNullException(nameof(postsResourceParameters));
            }

            var collection = _context.UserPosts as IQueryable<UserPost>;

            if (!string.IsNullOrWhiteSpace(postsResourceParameters.SearchQuery)) {
                var searchQuery = postsResourceParameters.SearchQuery.Trim();

                collection = collection.Where(x => x.Text.Contains(searchQuery) || x.Title.Contains(searchQuery));
            }


            return await PagedList<UserPost>.Create(collection, postsResourceParameters.PageNumber, postsResourceParameters.PageSize);
        }

        public async Task<bool> SaveChangesAsync() {
            return (await _context.SaveChangesAsync() > 0);
        }

    }
}
