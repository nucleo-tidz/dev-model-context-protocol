using infrastructure;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
namespace shipment.agents.Capacity
{

    public class BookingAgent(IMCPClientFactory clientFactory, IChatClient chatClient) : IAgent
    {
        public async Task<AIAgent> Create()
        {
            var client = await clientFactory.CreateBookingClient();
            var tools = await client.ListToolsAsync();
            return chatClient.AsAIAgent(new ChatClientAgentOptions
            {
                ChatOptions = new ChatOptions()
                {
                    Instructions = @"You are an AI agent tasked with creating a shipping container booking. You will receive the following details
                                  - Container Type (e.g., 20DRY)
                                  - Vessel ID
                                  - Origin City
                                  - Destination City
                                  Using this information, generate a valid booking for the container on the specified vessel between the given origin and destination.Ensure vessel has enough capacity to make the booking",
                    ToolMode = ChatToolMode.Auto,
                    Tools = [.. tools]
                },
                Description = "AI agent which creates a shipment booking on vessels",
                ChatHistoryProvider = new InMemoryChatHistoryProvider(),
            });
        }
    }
}
