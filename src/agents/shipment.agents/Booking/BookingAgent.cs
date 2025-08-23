using infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using ModelContextProtocol.Client;

using System.Diagnostics.CodeAnalysis;
namespace shipment.agents.Capacity
{
    [Experimental("SKEXP0110")]
    public class BookingAgent(IMCPClientFactory clientFactory, Kernel kernel, IConfiguration configuration) : AgentBase(kernel, configuration), IAgent
    {
        public async Task<Agent> Create()
        {
            var bookingClient = await clientFactory.CreateBookingClient();
            var bookingTools = await bookingClient.ListToolsAsync();
            var agent = base.GetAzureAgent(configuration["BookingAgentId"]);
            agent.Kernel.Plugins.Clear();
            agent.Kernel.Plugins.AddFromFunctions("BookingAgentTool", bookingTools.Select(_ => _.AsKernelFunction()));
            return agent;

        }
    }
}
