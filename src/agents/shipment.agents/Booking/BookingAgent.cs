using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using ModelContextProtocol.Client;
using shipment.agents.ClientFactory;
using System.Diagnostics.CodeAnalysis;
namespace shipment.agents.Capacity
{
    [Experimental("SKEXP0110")]
    public class BookingAgent
    {
        public ChatCompletionAgent Create(Kernel kernel)
        {
            var clientFactory = new MCPClientFactory();
            Kernel agentKernel = kernel.Clone();
            var bookingClient = clientFactory.CreateBookingClient().GetAwaiter().GetResult();
            var bookingTools = bookingClient.ListToolsAsync().GetAwaiter().GetResult();
            agentKernel.Plugins.AddFromFunctions("BookingAgentTool", bookingTools.Select(_ => _.AsKernelFunction()));
            return new ChatCompletionAgent()
            {
                Name = nameof(BookingAgent),
                Instructions = @" You are an AI agent tasked with creating a shipping container booking. You will receive the following details
                                  - Container Type (e.g., 20DRY)
                                  - Vessel ID
                                  - Origin City
                                  - Destination City
                                  Using this information, generate a valid booking for the container on the specified vessel between the given origin and destination.
                                ",
                Kernel = agentKernel,

                Arguments = new KernelArguments(new PromptExecutionSettings() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: new() { RetainArgumentTypes = true }) }),
            };
        }
    }
}
