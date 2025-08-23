using agent.speech.invoker;

using infrastructure;

using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using shipment.agents;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((ctx, cfg) =>
    {
        cfg.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);
        cfg.AddEnvironmentVariables();
    })
    .ConfigureServices((ctx, services) =>
    {      
        services.AddSemanticKernel(ctx.Configuration);
        services.AddMCPClientFactory();
        services.AddAzureTokenClient(ctx.Configuration);
        services.AddAgents();
        services.AddSpeech(ctx.Configuration);
#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        services.AddTransient<IBootStrapper, BootStrapper>();
#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    })
    .Build();

await using (host as IAsyncDisposable)
{
    var bs = host.Services.GetRequiredService<IBootStrapper>();
    await bs.StartGroupChatAsync();
}
