using CSharpFunctionalExtensions;
using MediatR;
using RESTfulApi_Reddit.Abstractions;
using RESTfulApi_Reddit.Services;
using System.Threading;
using System.Threading.Tasks;

namespace RESTfulApi_Reddit.AppServices.Commands
{
    public sealed class DeleteUserCommand : IRequest<int>
    {
        public readonly int UserId;

        public DeleteUserCommand(int userId)
        {
            UserId = userId;
        }


        internal sealed class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand,int>
        {
            private readonly IUserRepository _userRepository;

            public DeleteUserCommandHandler(IUserRepository userRepository)
            {
                _userRepository = userRepository;
            }

            public async Task<int> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
            {
                var userFromRepo = await _userRepository.GetUserAsync(request.UserId);

                if (userFromRepo == null)
                {
                    return 0;
                }

                _userRepository.DeleteUser(userFromRepo);

                await _userRepository.SaveChangesAsync();

                return request.UserId;
            }
        }
    }
}
