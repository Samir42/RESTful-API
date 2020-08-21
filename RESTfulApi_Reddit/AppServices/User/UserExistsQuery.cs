using MediatR;
using RESTfulApi_Reddit.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RESTfulApi_Reddit.AppServices.User
{
    public class UserExistsQuery : IRequest<bool>
    {
        private readonly int _userId;

        public UserExistsQuery(int userId)
        {
            _userId = userId;
        }


        internal sealed class UserExistsQueryHandler : IRequestHandler<UserExistsQuery, bool>
        {
            private readonly IUserRepository _userRepository;

            public UserExistsQueryHandler(IUserRepository userRepository)
            {
                _userRepository = userRepository;
            }

            public async Task<bool> Handle(UserExistsQuery request, CancellationToken cancellationToken)
            {
               return await _userRepository.UserExistsAsync(request._userId);
            }
        }
    }
}
