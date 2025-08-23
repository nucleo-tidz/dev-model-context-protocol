using infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using ModelContextProtocol.Client;
using System.Diagnostics.CodeAnalysis;
namespace shipment.agents.Capacity
{
    [Experimental("SKEXP0110")]
    public class CapacityAgent(IMCPClientFactory clientFactory, Kernel kernel, IConfiguration configuration) : AgentBase(kernel, configuration), IAgent
    {
        public async Task<Agent> Create()
        {
            var capacityClient = await clientFactory.CreateCapacityClient();
            var capacityTools = await capacityClient.ListToolsAsync();
            var agent = base.GetAzureAgent(configuration["CapacityAgentId"]);
            agent.Kernel.Plugins.Clear();
            agent.Kernel.Plugins.AddFromFunctions("CapacityContainerTool", capacityTools.Select(_ => _.AsKernelFunction()));
            return agent;
        }
    }
}
