using infrastructure;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using ModelContextProtocol.Client;
using shipment.agents.Vessel;

using System.Diagnostics.CodeAnalysis;
namespace shipment.agents.Capacity
{
    [Experimental("SKEXP0110")]
    public class CapacityAgent(IMCPClientFactory clientFactory): IAgent
    {
        public ChatCompletionAgent Create(Kernel kernel)
        {
       
            var capacityClient = clientFactory.CreateCapacityClient().GetAwaiter().GetResult();
            var capacityTools = capacityClient.ListToolsAsync().GetAwaiter().GetResult();

            return new ChatCompletionAgentBuilder()
            .WithKernel(kernel)
            .WithName(nameof(VesselAgent))
            .WithInstructions(@" You are an AI agent responsible for finding the space left on a vessel.You will be provided with a vessel id from the Vessel Agent ,Do not generate booking that is not your job")
            .WithDescription("an AI agent responsible for finding the space left on a vessel")
            .WithArgumnets(new KernelArguments(new PromptExecutionSettings() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: new() { RetainArgumentTypes = true }) }))
            .WithMCPPlugin("CapacityContainerTool", capacityTools.Select(_ => _.AsKernelFunction()))
            .Build();
        }
    }
}
