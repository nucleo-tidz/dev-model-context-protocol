using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.SemanticKernel;
namespace infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSemanticKernel(this IServiceCollection services, IConfiguration configuration, string connectorName = "AzureOpenAI")
        {
            return services.AddTransient<Kernel>(serviceProvider =>
            {
                IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
                if (connectorName == "OpenAI")
                {
                    kernelBuilder.Services.AddOpenAIChatCompletion(modelId: "openai/gpt-4o", endpoint: new Uri("https://models.github.ai/inference"), apiKey: "");
                }
                else if (connectorName == "AzureOpenAI")
                {
                    kernelBuilder.Services.AddAzureOpenAIChatCompletion("gpt-4o",
                      "https://lighthouse-ai.openai.azure.com/",
                       "",
                       "gpt-4o",
                       "gpt-4o");
                }
                return kernelBuilder.Build();
            });
        }
    }
}
