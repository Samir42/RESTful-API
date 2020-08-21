using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace RESTfulApi_Reddit.DI
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());
        }
    }
}
