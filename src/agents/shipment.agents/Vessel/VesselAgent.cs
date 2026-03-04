using infrastructure;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using shipment.agents.Capacity;

namespace shipment.agents.Vessel
{

    public class VesselAgent(IMCPClientFactory clientFactory,IChatClient chatClient): IAgent
    {
        public async Task<AIAgent> Create()
        {
            var client = await clientFactory.CreateVesselClient();
            var tools = await client.ListToolsAsync();
            return chatClient.AsAIAgent(new ChatClientAgentOptions
            {
                ChatOptions = new ChatOptions()
                {
                    Instructions = @"You are an AI agent responsible for vessel operations: finding vessels and locking space.
                                                          
                               If NO vessel has been found yet:
                               - ONLY use GetVessel to find a vessel between origin and destination
                               - Do NOT lock space in the same turn
                               - Stop after finding the vessel                               
                               If a vessel HAS been found AND capacity has been confirmed:
                               - ONLY use LockVesselSpace to lock space on the vessel
                               - Use the VesselId from the conversation history
                               - Stop after locking space                               
                               Do not check capacity or create bookings - that is not your job.",
                    ToolMode = ChatToolMode.Auto,
                    Tools = [.. tools]
                },
                Description = "AI agent responsible for searching vessel between origin and destination and locking space on vessel",
                ChatHistoryProvider = new InMemoryChatHistoryProvider(),
                Name = nameof(VesselAgent),
            });
        }

    }
}
