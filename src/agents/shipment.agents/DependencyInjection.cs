using Microsoft.Extensions.DependencyInjection;
using shipment.agents.Capacity;
using shipment.agents.Group;
using shipment.agents.Vessel;
using System.Diagnostics.CodeAnalysis;
namespace shipment.agents
{
    [Experimental("SKEXP0110")]
    public static class DependencyInjection
    {
        public static IServiceCollection AddAgents(this IServiceCollection services)
        {
            return services.AddTransient<IAgent, VesselAgent>()
                .AddTransient<IAgent, CapacityAgent>()
                .AddTransient<IAgent, BookingAgent>().AddTransient<IGroupAgent, GroupAgent>();
        }
    }
}

