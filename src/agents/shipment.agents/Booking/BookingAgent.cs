using infrastructure;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using ModelContextProtocol.Client;

using System.Diagnostics.CodeAnalysis;
namespace shipment.agents.Capacity
{
    [Experimental("SKEXP0110")]
    public class BookingAgent(IMCPClientFactory clientFactory): IAgent
    {
        public ChatCompletionAgent Create(Kernel kernel)
        {  
            var bookingClient = clientFactory.CreateBookingClient().GetAwaiter().GetResult();
            var bookingTools = bookingClient.ListToolsAsync().GetAwaiter().GetResult(); 
            return   new ChatCompletionAgentBuilder()
                .WithKernel(kernel)
                .WithName(nameof(BookingAgent))
                .WithInstructions(@" You are an AI agent tasked with creating a shipping container booking. You will receive the following details
                                  - Container Type (e.g., 20DRY)
                                  - Vessel ID
                                  - Origin City
                                  - Destination City
                                  Using this information, generate a valid booking for the container on the specified vessel between the given origin and destination.Ensure vessel has enough capacity to make the booking")
                .WithDescription("AI agent which creates a shipment booking on vessels")
                .WithArgumnets(new KernelArguments(new PromptExecutionSettings() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: new() { RetainArgumentTypes = true }) }))
                .WithMCPPlugin("BookingAgentTool", bookingTools)
                .Build();
   
        }
    }
}
