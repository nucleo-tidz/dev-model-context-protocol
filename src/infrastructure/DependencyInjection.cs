using infrastructure.Service;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAgents(this IServiceCollection services, IConfiguration configuration, string connectorName = "o4-mini")
        {
            var client = new Azure.AI.OpenAI.AzureOpenAIClient(new Uri(configuration["foundryurl"]),
                new System.ClientModel.ApiKeyCredential(configuration["foundrykey"]));

            services.AddChatClient(client.GetChatClient("gpt-4.1").AsIChatClient());
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

