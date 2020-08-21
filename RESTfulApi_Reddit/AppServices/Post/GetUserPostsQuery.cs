using MediatR;
using RESTfulApi_Reddit.Entities;
using RESTfulApi_Reddit.Helpers;
using RESTfulApi_Reddit.ResourceParameter;
using RESTfulApi_Reddit.Services;
using System.Threading;
using System.Threading.Tasks;

namespace RESTfulApi_Reddit.AppServices.Post
{
    public class GetUserPostsQuery : IRequest<PagedList<UserPost>>
    {
        private readonly ResourceParameters postsResourceParameters;

        public GetUserPostsQuery(ResourceParameters postsResourceParameters)
        {
            this.postsResourceParameters = postsResourceParameters;
        }

        internal sealed class GetUserPostsQueryHandler : IRequestHandler<GetUserPostsQuery, PagedList<UserPost>>
        {
            private readonly IPostRepository _postRepository;

            public GetUserPostsQueryHandler(IPostRepository postRepository)
            {
                _postRepository = postRepository;
            }

            public async Task<PagedList<UserPost>> Handle(GetUserPostsQuery request, CancellationToken cancellationToken)
            {
               var userPostsFromRepo =  await _postRepository.GetUserPostsAsync(request.postsResourceParameters);

                return userPostsFromRepo;
            }
        }
    }
}
