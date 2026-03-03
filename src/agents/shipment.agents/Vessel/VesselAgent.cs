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
                    Instructions = @"You are an AI agent responsible for searching vessel between origin and destination.You will be provided with an origin city name and destination city name, 
                                  Do not assume or guess the origin or destination city name if it is not explicitly provided , Do not check capacity or generate booking that is not your job.
                               
                               Your workflow includes one steps:
                               1. Find vessel between origin and destination city name..",
                    ToolMode = ChatToolMode.Auto,
                    Tools = [.. tools]
                },
                Description = "AI agent responsible for searching vessel between origin and destination",
                ChatHistoryProvider = new InMemoryChatHistoryProvider(),
                Name = nameof(VesselAgent),
            });
        }

    }
}
