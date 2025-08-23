namespace shipment.agents.Group
{
    using agents.Orchestrator;

    using Microsoft.Extensions.Logging;
    using Microsoft.SemanticKernel;
    using Microsoft.SemanticKernel.Agents.Orchestration;
    using Microsoft.SemanticKernel.Agents.Orchestration.GroupChat;
    using Microsoft.SemanticKernel.ChatCompletion;

    using System.Diagnostics.CodeAnalysis;

    [Experimental("SKEXP0110")]
    public class GroupAgent(IEnumerable<IAgent> agents, Kernel kernel, ILogger<ShipmnetGroupManager> logger) : IGroupAgent
    {
        public async Task<GroupChatOrchestration> CreateAgentGroupChat(OrchestrationResponseCallback responseCallback, OrchestrationInteractiveCallback orchestrationInteractiveCallback)
        {
            var shipmentAgents = await Task.WhenAll(agents.Select(agent => agent.Create()));

            var groupManager = new ShipmnetGroupManager(kernel.GetRequiredService<IChatCompletionService>(), logger)
            {
                MaximumInvocationCount = 4,
                InteractiveCallback = orchestrationInteractiveCallback
            };
            return new GroupChatOrchestration(groupManager, shipmentAgents.ToArray())
            {
                ResponseCallback = responseCallback,

            };
        }
    }
}
