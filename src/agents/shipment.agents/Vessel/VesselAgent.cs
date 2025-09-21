using infrastructure;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

using ModelContextProtocol.Client;

using shipment.agents.Capacity;

using System.Diagnostics.CodeAnalysis;

namespace shipment.agents.Vessel
{
    [Experimental("SKEXP0110")]
    public class VesselAgent(IMCPClientFactory clientFactory) : IAgent
    {
        public ChatCompletionAgent Create(Kernel kernel)
        {
            var vesselClient = clientFactory.CreateVesselClient().GetAwaiter().GetResult();
            var vesselTools = vesselClient.ListToolsAsync().GetAwaiter().GetResult();
            return new ChatCompletionAgentBuilder()
             .WithKernel(kernel)
             .WithName(nameof(VesselAgent))
             .WithInstructions(@"You are an AI agent responsible for searching vessel between origin and destination.You will be provided with an origin city name and destination city name, 
                                  Do not assume or guess the origin or destination city name if it is not explicitly provided , Do not check capacity or generate booking that is not your job.
                               
                               Your workflow includes one steps:
                               1. Find vessel between origin and destination city name..")
             .WithDescription("AI agent responsible for searching vessel between origin and destination")
             .WithArgumnets(new KernelArguments(new PromptExecutionSettings() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: new() { RetainArgumentTypes = true }) }))
             .WithMCPPlugin("VesselContainerTool", vesselTools)
             .Build();

        }

    }
}
