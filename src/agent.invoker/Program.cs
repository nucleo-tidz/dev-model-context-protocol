using client;

using infrastructure;

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
#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        services.AddTransient<IBootStrapper, BootStrapper>();
     
#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    }).Build();

IBootStrapper aiservice = host.Services.GetRequiredService<IBootStrapper>();
aiservice.Run().Wait();