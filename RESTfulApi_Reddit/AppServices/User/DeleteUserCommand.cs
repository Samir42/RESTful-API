using CSharpFunctionalExtensions;
using RESTfulApi_Reddit.Abstractions;
using RESTfulApi_Reddit.Services;
using System.Threading.Tasks;

namespace RESTfulApi_Reddit.AppServices.User
{
    public sealed class DeleteUserCommand : ICommand
    {
        public readonly int UserId;

        public DeleteUserCommand(int userId)
        {
            UserId = userId;
        }


        internal sealed class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
        {
            private readonly IUserRepository _userRepository;

            public DeleteUserCommandHandler(IUserRepository userRepository)
            {
                _userRepository = userRepository;
            }

            public async Task<Result> Handle(DeleteUserCommand command)
            {
                var userFromRepo = await _userRepository.GetUserAsync(command.UserId);

                if (userFromRepo == null)
                {
                    return Result.Failure($"No user found for {command.UserId}");
                }

                _userRepository.DeleteUser(userFromRepo);

                await _userRepository.SaveChangesAsync();

                return Result.Success();
            }
        }
    }
}
