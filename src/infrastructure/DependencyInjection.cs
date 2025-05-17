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
            {//AIzaSyB__RPh0X68ufHHPE9OLHnlSq4UDnz1z4c
                IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                kernelBuilder.Services.AddGoogleAIGeminiChatCompletion("gemini-2.0-flash", configuration["GeminiKey"], serviceId: "gpt-4-turbo");
#pragma warning restore SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.       
                return kernelBuilder.Build();
            });
        }

    }
}
