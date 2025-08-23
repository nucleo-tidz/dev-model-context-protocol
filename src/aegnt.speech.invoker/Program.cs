using client;

using infrastructure;

using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using shipment.agents;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((config, services) =>
    {
        services.AddSemanticKernel(config.Configuration);
        services.AddMCPClientFactory();
        services.AddAzureTokenClient(config.Configuration);
        services.AddAgents();

#pragma warning disable SKEXP0110

        services.AddTransient<IBootStrapper, BootStrapper>();

        // ✅ Correct SpeechConfig registration
        services.AddSingleton(sp =>
        {
            var speechConfig = SpeechConfig.FromEndpoint(
                new Uri("https://centralindia.api.cognitive.microsoft.com/"),
                config.Configuration["SpeechKey"]);
            speechConfig.SpeechRecognitionLanguage = "en-IN";
            return speechConfig; // ✅ FIXED (was returning config)
        });

        // ✅ Register SpeechRecognizer
        services.AddTransient(sp =>
        {
            var speechConfig = sp.GetRequiredService<SpeechConfig>();
            var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            return new SpeechRecognizer(speechConfig, audioConfig);
        });

        // ✅ Register SpeechSynthesizer
        services.AddTransient(sp =>
        {
            var speechConfig = sp.GetRequiredService<SpeechConfig>();
            speechConfig.SpeechSynthesisVoiceName = "en-US-JennyNeural";
            return new SpeechSynthesizer(speechConfig);
        });

#pragma warning restore SKEXP0110
    })
    .Build();

IBootStrapper bootStrapper = host.Services.GetRequiredService<IBootStrapper>();
await bootStrapper.StartGroupChatAsync(CancellationToken.None);
