using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using ModelContextProtocol.Client;
using shipment.agents.ClientFactory;
using System.Diagnostics.CodeAnalysis;

namespace shipment.agents.Vessel
{
    [Experimental("SKEXP0110")]
    public class VesselAgent
    {
        public  ChatCompletionAgent Create(Kernel kernel)
        {
            var clientFactory = new MCPClientFactory();
            Kernel agentKernel = kernel.Clone();
            var vesselClient = clientFactory.CreateVesselClient().GetAwaiter().GetResult();
            var vesselTools = vesselClient.ListToolsAsync().GetAwaiter().GetResult();
            agentKernel.Plugins.AddFromFunctions("VesselContainerTool", vesselTools.Select(_ => _.AsKernelFunction())); 
            return new ChatCompletionAgent()
            {
                Name = nameof(VesselAgent),
                Instructions = @" You are an AI agent responsible for searching vessel between origin and destination.You will be provided with an origin city name and destination city name, Do not assume or guess the origin or destination city name if it is not explicitly provided.
                               
                               Your workflow includes one steps:
                               1. Find vessel between origin and destination city name.",
                Kernel = agentKernel,
             
                Arguments = new KernelArguments(new PromptExecutionSettings() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: new() { RetainArgumentTypes = true }) }),
            };
        }
    }
}
