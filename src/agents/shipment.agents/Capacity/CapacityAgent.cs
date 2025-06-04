using infrastructure;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using ModelContextProtocol.Client;
using System.Diagnostics.CodeAnalysis;
namespace shipment.agents.Capacity
{
    [Experimental("SKEXP0110")]
    public class CapacityAgent(IMCPClientFactory clientFactory): IAgent
    {
        public ChatCompletionAgent Create(Kernel kernel)
        {
            Kernel agentKernel = kernel.Clone();
            var capacityClient = clientFactory.CreateCapacityClient().GetAwaiter().GetResult();
            var capacityTools = capacityClient.ListToolsAsync().GetAwaiter().GetResult();
            agentKernel.Plugins.AddFromFunctions("CapacityContainerTool", capacityTools.Select(_ => _.AsKernelFunction()));
            return new ChatCompletionAgent()
            {
                Name = nameof(CapacityAgent),
                Instructions = @" You are an AI agent responsible for finding the space left on a vessel.You will be provided with a vessel id from the Vessel Agent ,Do not generate booking that is not your job",
                Kernel = agentKernel,
                Description = "an AI agent responsible for finding the space left on a vessel",

                Arguments = new KernelArguments(new PromptExecutionSettings() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: new() { RetainArgumentTypes = true }) }),
            };
        }
    }
}
