using MediatR;
using RESTfulApi_Reddit.Entities;
using RESTfulApi_Reddit.Services;
using System.Threading;
using System.Threading.Tasks;

namespace RESTfulApi_Reddit.AppServices.Post
{
    public class GetUserPostByUserAndPostIdQuery : IRequest<UserPost>
    {
        private readonly int UserId;
        private readonly int UserPostId;

        public GetUserPostByUserAndPostIdQuery(int userId, int userPostId)
        {
            UserId = userId;
            UserPostId = userPostId;
        }


        internal sealed class GetUserPostByUserAndPostIdQueryHandler : IRequestHandler<GetUserPostByUserAndPostIdQuery, UserPost>
        {
            private readonly IUserRepository _userRepository;
            private readonly IPostRepository _postRepository;

            public GetUserPostByUserAndPostIdQueryHandler(IUserRepository userRepository, IPostRepository postRepository)
            {
                _userRepository = userRepository;
                _postRepository = postRepository;
            }

            public async Task<UserPost> Handle(GetUserPostByUserAndPostIdQuery request, CancellationToken cancellationToken)
            {
                var userFromRepositoryToReturn = await _postRepository.GetUserPostAsync(request.UserId, request.UserPostId);

                return userFromRepositoryToReturn;
            }
        }
    }
}
