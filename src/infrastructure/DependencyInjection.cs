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


                kernelBuilder.Services.AddAzureOpenAIChatCompletion("o4-mini",
                  configuration["o4-mini-url"],
                  configuration["o4-mini"],
                   "o4-mini",
                   "o4-mini");



                //kernelBuilder.Services.AddAzureOpenAIChatCompletion("gpt-4.1-mini",
                //  "https://nucle-mbdqap7c-southeastasia.openai.azure.com/",
                // "Bc67jEum8xcDZmJ5pWopes1wdfgwCNAQ0MukJLCcz1wSs7aorsy0JQQJ99BFACqBBLyXJ3w3AAAAACOGQakB",
                //   "o4-mini",
                //   "o4-mini");

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

