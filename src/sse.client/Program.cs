
using infrastructure;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using sse.client;
IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((config, services) =>
    {
        services.AddSemanticKernel(config.Configuration);
        services.AddTransient<IBootStrapper, BootStrapper>();
    }).Build();

IBootStrapper aiservice = host.Services.GetRequiredService<IBootStrapper>();
aiservice.Run().Wait();