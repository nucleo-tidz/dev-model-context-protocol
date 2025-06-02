namespace shipment.agents.Group
{
    using agents.Orchestrator;
    using Microsoft.SemanticKernel;
    using Microsoft.SemanticKernel.Agents.Orchestration;
    using Microsoft.SemanticKernel.Agents.Orchestration.GroupChat;
    using Microsoft.SemanticKernel.ChatCompletion;
    using shipment.agents.Capacity;
    using shipment.agents.Vessel;
    using System.Diagnostics.CodeAnalysis;

    [Experimental("SKEXP0110")]
    public class GroupAgent()
    {
        public GroupChatOrchestration CreateAgentGroupChat(Kernel kernel, OrchestrationResponseCallback responseCallback)
        {

            var vesselAgent = new VesselAgent().Create(kernel);
            var capacityAgent = new CapacityAgent().Create(kernel);
            var bookingAgent = new BookingAgent().Create(kernel);
            GroupChatOrchestration orchestration = new(
            new ShipmnetGroupManager(kernel.GetRequiredService<IChatCompletionService>()) { MaximumInvocationCount = 4 }, vesselAgent, capacityAgent, bookingAgent)
            {
                ResponseCallback = responseCallback,

            };
            return orchestration;
        }

    }
}
