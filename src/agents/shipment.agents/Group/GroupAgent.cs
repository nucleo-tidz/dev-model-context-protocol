namespace shipment.agents.Group
{
    using agents.Orchestrator;
    using Microsoft.SemanticKernel;
    using Microsoft.SemanticKernel.Agents;
    using Microsoft.SemanticKernel.Agents.Orchestration;
    using Microsoft.SemanticKernel.Agents.Orchestration.GroupChat;
    using Microsoft.SemanticKernel.ChatCompletion;
    using OpenAI.Chat;
    using shipment.agents.Capacity;
    using shipment.agents.Vessel;
    using System.Diagnostics.CodeAnalysis;

    [Experimental("SKEXP0110")]
    public class GroupAgent(IEnumerable<IAgent> agents, Kernel kernel) : IGroupAgent
    {
        public GroupChatOrchestration CreateAgentGroupChat( OrchestrationResponseCallback responseCallback)
        {
            List<ChatCompletionAgent> shipmentagents = new List<ChatCompletionAgent>();
            foreach (var agent in agents) 
            {
                shipmentagents.Add(agent.Create(kernel));
            }
            GroupChatOrchestration orchestration = new(
            new ShipmnetGroupManager(kernel.GetRequiredService<IChatCompletionService>()) { MaximumInvocationCount = 4 }, shipmentagents.ToArray())
            {
                ResponseCallback = responseCallback,
            };
            return orchestration;
        }

    }
}
