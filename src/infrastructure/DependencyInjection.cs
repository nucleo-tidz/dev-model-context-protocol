using infrastructure.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using static infrastructure.Service.AccessTokenService;
namespace infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSemanticKernel(this IServiceCollection services, IConfiguration configuration, string connectorName = "o4-mini")
        {
            return services.AddTransient<Kernel>(serviceProvider =>
            {
                IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
                if (connectorName == "OpenAI")
                {
                    kernelBuilder.Services.AddOpenAIChatCompletion(modelId: "openai/gpt-4o", endpoint: new Uri("https://models.github.ai/inference"), apiKey: configuration["GitAI"]);
                }
                else if (connectorName == "o4-mini")
                {
                    kernelBuilder.Services.AddAzureOpenAIChatCompletion("o4-mini",
                      configuration["o4-mini-url"],
                      configuration["o4-mini"],
                       "o4-mini",
                       "o4-mini");
                }
                else if (connectorName == "AzureOpenAI")
                {
                    kernelBuilder.Services.AddAzureOpenAIChatCompletion("gpt-4o",
                        configuration["gpt-4o-url"],
                        configuration["gpt-4o"],
                       "gpt-4o",
                       "gpt-4o");
                }
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

