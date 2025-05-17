using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection Add(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }

    }
}
