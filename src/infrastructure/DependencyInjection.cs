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
            {//https://www.linkedin.com/pulse/mcp-net-c-right-way-hosted-aspnetcore-aspire-what-else-latorre-zs3ff/
                IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
                kernelBuilder.Services.AddAzureOpenAIChatCompletion("gpt-4o",
                   "https://lighthouse-ai.openai.azure.com/",
                   "",
                   "gpt-4o",
                   "gpt-4o");
                return kernelBuilder.Build();
            });
        }

    }
}
