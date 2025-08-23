using infrastructure.Service;
using Microsoft.CognitiveServices.Speech.Audio;

using Microsoft.CognitiveServices.Speech;
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
                kernelBuilder.Services.AddAzureOpenAIChatCompletion("o4-mini",
                  configuration["o4-mini-url"],
                  configuration["o4-mini"],
                   "o4-mini",
                   "o4-mini");
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
        public static IServiceCollection AddSpeech(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(sp =>
            {
                var endpoint = configuration["Speech:Endpoint"];
                var key = configuration["Speech:Key"];

                // Use FromEndpoint if you really have a full endpoint URL in config
                var config = SpeechConfig.FromEndpoint(new Uri(endpoint!), key);

                config.SpeechRecognitionLanguage = "en-IN";
                config.SpeechSynthesisVoiceName = "en-US-JennyNeural";
                return config;
            });

            // Factory for creating new AudioConfig when needed
            services.AddSingleton<Func<AudioConfig>>(_ =>
                () => AudioConfig.FromDefaultMicrophoneInput()
            );

            // Register services that depend on SpeechConfig + AudioConfig
            services.AddSingleton<ISpeechService, SpeechService>();
            services.AddSingleton<IApprovalService, ApprovalService>();

            return services;
        }
    }
}

