using MediatR;
using RESTfulApi_Reddit.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RESTfulApi_Reddit.AppServices.Commands
{
    public class DeleteUserPostCommand : IRequest<int>
    {
        public int UserPostId { get; private set; }

        public DeleteUserPostCommand(int userPostId)
        {
            UserPostId = userPostId;
        }

        internal sealed class DeleteUserPostCommandHandler : IRequestHandler<DeleteUserPostCommand, int>
        {
            private IPostRepository _postRepository;

            public DeleteUserPostCommandHandler(IPostRepository postRepository)
            {
                _postRepository = postRepository;
            }

            public async Task<int> Handle(DeleteUserPostCommand request, CancellationToken cancellationToken)
            {
                var userPost = await _postRepository.GetUserPostAsync(request.UserPostId);

                if (userPost == null)
                {
                    throw new KeyNotFoundException($"No userPost found for {request.UserPostId}");
                }

                _postRepository.DeleteUserPost(userPost);

                await _postRepository.SaveChangesAsync();

                return request.UserPostId;
            }
        }
    }
}
