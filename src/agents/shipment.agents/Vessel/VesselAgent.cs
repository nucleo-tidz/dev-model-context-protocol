using infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using ModelContextProtocol.Client;
using System.Diagnostics.CodeAnalysis;

namespace shipment.agents.Vessel
{
    [Experimental("SKEXP0110")]
    public class VesselAgent(IMCPClientFactory clientFactory, Kernel kernel, IConfiguration configuration) : AgentBase(kernel, configuration), IAgent
    {
        public async Task<Agent> Create()
        {
            var vesselClient = await clientFactory.CreateVesselClient();
            var vesselTools = await vesselClient.ListToolsAsync();
            var agent = base.GetAzureAgent(configuration["VesselAgentId"]);
            agent.Kernel.Plugins.Clear();
            agent.Kernel.Plugins.AddFromFunctions("VesselContainerTool", vesselTools.Select(_ => _.AsKernelFunction()));
            return agent;
        }
    }
}
