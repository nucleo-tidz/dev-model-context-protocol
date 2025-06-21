namespace shipment.agents.Group
{
    using agents.Orchestrator;
    using Microsoft.SemanticKernel;
    using Microsoft.SemanticKernel.Agents;
    using Microsoft.SemanticKernel.Agents.Orchestration;
    using Microsoft.SemanticKernel.Agents.Orchestration.GroupChat;
    using Microsoft.SemanticKernel.ChatCompletion;
    using System.Diagnostics.CodeAnalysis;

    [Experimental("SKEXP0110")]
    public class GroupAgent(IEnumerable<IAgent> agents, Kernel kernel) : IGroupAgent
    {
        public GroupChatOrchestration CreateAgentGroupChat( OrchestrationResponseCallback responseCallback)
        {
            var shipmentAgents = agents.Select(agent => agent.CreateAgents(kernel)).ToList();
            var groupManager = new ShipmnetGroupManager(kernel.GetRequiredService<IChatCompletionService>())
            {
                MaximumInvocationCount = 4, 
                
            };
            return new GroupChatOrchestration(groupManager, shipmentAgents.ToArray())
            {
                ResponseCallback = responseCallback,
                
            };
        }
    }
}
