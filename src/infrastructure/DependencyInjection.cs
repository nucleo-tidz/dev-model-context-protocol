using infrastructure.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
namespace infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSemanticKernel(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddTransient<Kernel>(serviceProvider =>
            {
                IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
                kernelBuilder.Services.AddAzureOpenAIChatCompletion(deploymentName:"o4-mini",
                  endpoint:configuration["o4-mini-url"],
                  apiKey:configuration["o4-mini"],
                  serviceId: "o4-mini",
                   modelId:"o4-mini");
                return kernelBuilder.Build();


            });
        }
        public static IServiceCollection AddAzureTokenClient(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                           .AddTransient<IAccessTokenService, AccessTokenService>()
                           .AddHttpClient();
        }
        public static IServiceCollection AddMCPClientFactory(this IServiceCollection services)
        {
            return services.AddTransient<IMCPClientFactory, MCPClientFactory>();
        }
    }
}

