using MediatR;
using RESTfulApi_Reddit.Entities;
using RESTfulApi_Reddit.Services;
using System.Threading;
using System.Threading.Tasks;

namespace RESTfulApi_Reddit.AppServices.Post
{
    public class GetUserPostByIdQuery : IRequest<UserPost>
    {
        private readonly int _userPostId;

        public GetUserPostByIdQuery(int userPostId)
        {
            _userPostId = userPostId;
        }

        internal sealed class GetUserPostQueryHandler : IRequestHandler<GetUserPostByIdQuery, UserPost>
        {
            private IPostRepository _postRepository;

            public GetUserPostQueryHandler(IPostRepository postRepository)
            {
                _postRepository = postRepository;
            }
            public async Task<UserPost> Handle(GetUserPostByIdQuery request, CancellationToken cancellationToken)
            {
                var userPostFromRepoToReturn = await _postRepository.GetUserPostAsync(request._userPostId);

                return userPostFromRepoToReturn;
            }
        }
    }
}
