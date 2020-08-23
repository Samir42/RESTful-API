using MediatR;
using RESTfulApi_Reddit.Abstractions;
using RESTfulApi_Reddit.Services;
using System.Threading;
using System.Threading.Tasks;

namespace RESTfulApi_Reddit.AppServices.Queries
{
    public sealed class GetUserQuery : IRequest<Entities.User>
    {
        public readonly int UserId;

        public GetUserQuery(int userId)
        {
            UserId = userId;
        }

        internal sealed class GetUserQueryHandler : IRequestHandler<GetUserQuery, Entities.User>
        {
            private readonly IUserRepository _userRepository;

            public GetUserQueryHandler(IUserRepository userRepository)
            {
                _userRepository = userRepository;
            }

            public  async Task<Entities.User> Handle(GetUserQuery request, CancellationToken cancellationToken)
            {
                var userFromRepo = await _userRepository.GetUserAsync(request.UserId);

                return userFromRepo;
            }
        }
    }
}
