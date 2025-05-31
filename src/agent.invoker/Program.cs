using client;

using infrastructure;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((config, services) =>
    {
        services.AddSemanticKernel(config.Configuration);
#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        services.AddTransient<IBootStrapper, BootStrapper>();
#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    }).Build();

IBootStrapper aiservice = host.Services.GetRequiredService<IBootStrapper>();
aiservice.Run().Wait();