using RESTfulApi_Reddit.Abstractions;
using RESTfulApi_Reddit.Services;
using System.Threading.Tasks;

namespace RESTfulApi_Reddit.AppServices.User
{
    public sealed class GetUserQuery : IQuery<Entities.User>
    {
        public readonly int UserId;

        public GetUserQuery(int userId)
        {
            UserId = userId;
        }


        internal sealed class GetUserQueryHandler : IQueryHandler<GetUserQuery, Entities.User>
        {
            private readonly IUserRepository _userRepository;

            public GetUserQueryHandler(IUserRepository userRepository)
            {
                _userRepository = userRepository;
            }


            public async Task<Entities.User> Handle(GetUserQuery query)
            {
                var userFromRepo = await _userRepository.GetUserAsync(query.UserId);

                return userFromRepo;
            }
        }
    }
}
