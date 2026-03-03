using infrastructure.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAgents(this IServiceCollection services, IConfiguration configuration, string connectorName = "o4-mini")
        {
            return services;
        }
        public static IServiceCollection AddAzureTokenClient(this IServiceCollection services, IConfiguration configuration)
        {
           return services.AddMemoryCache()
                          .AddTransient<IAccessTokenService, AccessTokenService>()
                          .AddHttpClient();
        }
        public static IServiceCollection AddMCPClientFactory(this IServiceCollection services)
        {           
            return services.AddTransient<IMCPClientFactory, MCPClientFactory>();
        }        
    }
}

