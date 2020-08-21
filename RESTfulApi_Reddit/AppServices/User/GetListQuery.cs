using RESTfulApi_Reddit.Abstractions;
using RESTfulApi_Reddit.Helpers;
using RESTfulApi_Reddit.ResourceParameter;
using RESTfulApi_Reddit.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RESTfulApi_Reddit.AppServices.User
{
    public class GetListQuery : IQuery<PagedList<Entities.User>>
    {
        public readonly ResourceParameters ResourceParameters;

        public GetListQuery(ResourceParameters resourceParameters)
        {
            ResourceParameters = resourceParameters;
        }


        internal class GetListQueryHandler : IQueryHandler<GetListQuery, PagedList<Entities.User>>
        {
            private readonly IUserRepository _userRepository;

            public GetListQueryHandler(IUserRepository userRepository)
            {
                _userRepository = userRepository;
            }

            public async Task<PagedList<Entities.User>> Handle(GetListQuery query)
            {
                var usersFromRepo = await _userRepository.GetUsersAsync(query.ResourceParameters);

                return usersFromRepo;
            }
        }
    }
}
