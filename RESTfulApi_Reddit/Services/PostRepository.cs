using Microsoft.EntityFrameworkCore;
using RESTfulApi_Reddit.DbContexts;
using RESTfulApi_Reddit.Entities;
using RESTfulApi_Reddit.Helpers;
using RESTfulApi_Reddit.ResourceParameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RESTfulApi_Reddit.Services
{
    public class PostRepository : IPostRepository
    {

        private RedditDbContext _context;

        public PostRepository(RedditDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<UserPost> GetUserPostAsync(int userPostId)
        {
            if (userPostId <= 0)
            {
                throw new ArgumentException("userPostId can not be less than 1");
            }

            return await _context.UserPosts.FirstOrDefaultAsync(x => x.Id == userPostId);
        }

        public async Task<UserPost> GetUserPostAsync(int userId,int userPostId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("userId can not be less than 1");
            }

            if (userPostId <= 0)
            {
                throw new ArgumentException("userPostId can not be less than 1");
            }

            return await _context.UserPosts.FirstOrDefaultAsync(x => x.Id == userPostId && x.UserId == userId);
        }

        public async Task<PagedList<UserPost>> GetUserPostsAsync(ResourceParameters postsResourceParameters)
        {

            if (postsResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(postsResourceParameters));
            }

            var collection = _context.UserPosts as IQueryable<UserPost>;

            if (!string.IsNullOrWhiteSpace(postsResourceParameters.SearchQuery))
            {
                var searchQuery = postsResourceParameters.SearchQuery.Trim();

                collection = collection.Where(x => x.Text.Contains(searchQuery) || x.Title.Contains(searchQuery));
            }


            return await PagedList<UserPost>.Create(collection, postsResourceParameters.PageNumber, postsResourceParameters.PageSize);
        }

        public void DeleteUserPost(UserPost userPost)
        {
            if (userPost == null)
            {
                throw new ArgumentNullException(nameof(userPost));
            }


            _context.Remove(userPost);
        }

        public void UpdateUserPost(UserPost userPost)
        {
            // no code implementation here. Because EF Core follows entities
        }

        public void AddUserPost(int userId, UserPost userPost)
        {
            if (userPost == null)
            {
                throw new ArgumentNullException(nameof(userPost));
            }

            userPost.UserId = userId;
            _context.UserPosts.Add(userPost);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() > 0);
        }

    }
}
