using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents.Orchestration;
using Microsoft.SemanticKernel.Agents.Orchestration.GroupChat;
using System.Diagnostics.CodeAnalysis;

namespace shipment.agents.Group
{
    [Experimental("SKEXP0110")]
    public interface IGroupAgent
    {
        GroupChatOrchestration CreateAgentGroupChat( OrchestrationResponseCallback responseCallback);
    }
}
