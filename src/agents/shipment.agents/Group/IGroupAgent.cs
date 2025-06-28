using Microsoft.SemanticKernel.Agents.Orchestration;
using Microsoft.SemanticKernel.Agents.Orchestration.GroupChat;
using System.Diagnostics.CodeAnalysis;

namespace shipment.agents.Group
{
    [Experimental("SKEXP0110")]
    public interface IGroupAgent
    {
        Task<GroupChatOrchestration> CreateAgentGroupChat(OrchestrationResponseCallback responseCallback, OrchestrationInteractiveCallback orchestrationInteractiveCallback);
    }
}
