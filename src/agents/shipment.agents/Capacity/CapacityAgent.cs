using infrastructure;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
namespace shipment.agents.Capacity
{

    public class CapacityAgent(IMCPClientFactory clientFactory,IChatClient chatClient): IAgent
    {
        public async Task<AIAgent> Create()
        {
            var client = await clientFactory.CreateCapacityClient();
            var tools = await client.ListToolsAsync();
            return chatClient.AsAIAgent(new ChatClientAgentOptions
            {
                ChatOptions = new ChatOptions()
                {
                    Instructions = @"You are an AI agent responsible for finding the space left on a vessel.You will be provided with a vessel id from the Vessel Agent ,
                                    Do not generate booking that is not your job",
                    ToolMode = ChatToolMode.Auto,
                    Tools = [.. tools]
                },
                Description = "AI agent responsible for finding the space left on a vessel",
                ChatHistoryProvider = new InMemoryChatHistoryProvider(),
            });
        }
    }
}
