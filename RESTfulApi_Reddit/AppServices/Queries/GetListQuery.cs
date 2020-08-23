using MediatR;
using RESTfulApi_Reddit.Helpers;
using RESTfulApi_Reddit.ResourceParameter;
using RESTfulApi_Reddit.Services;
using System.Threading;
using System.Threading.Tasks;

namespace RESTfulApi_Reddit.AppServices.Queries
{
    public class GetListQuery : IRequest<PagedList<Entities.User>>
    {
        public readonly ResourceParameters ResourceParameters;

        public GetListQuery(ResourceParameters resourceParameters)
        {
            ResourceParameters = resourceParameters;
        }


        internal class GetListQueryHandler : IRequestHandler<GetListQuery, PagedList<Entities.User>>
        {
            private readonly IUserRepository _userRepository;

            public GetListQueryHandler(IUserRepository userRepository)
            {
                _userRepository = userRepository;
            }

            public async Task<PagedList<Entities.User>> Handle(GetListQuery request, CancellationToken cancellationToken)
            {
                var usersFromRepo = await _userRepository.GetUsersAsync(request.ResourceParameters);

                return usersFromRepo;
            }
        }
    }
}
