namespace shipment.agents.Group
{
    using agents.Orchestrator;
    using Microsoft.SemanticKernel;
    using Microsoft.SemanticKernel.Agents;
    using Microsoft.SemanticKernel.Agents.Orchestration;
    using Microsoft.SemanticKernel.Agents.Orchestration.GroupChat;
    using Microsoft.SemanticKernel.ChatCompletion;
    using shipment.agents.Capacity;
    using shipment.agents.Vessel;
    using System.Diagnostics.CodeAnalysis;

    [Experimental("SKEXP0110")]
    public class GroupAgent()
    {
        public AgentGroupChat CreateAgentGroupChat(Kernel kernel)
        {

            var vesselAgent = new VesselAgent().Create(kernel);
            var capacityAgent = new CapacityAgent().Create(kernel);
            var bookingAgent = new BookingAgent().Create(kernel);
            var chatOrchestrator = new Orchestrator(kernel);

            return new AgentGroupChat(vesselAgent, capacityAgent, bookingAgent)
            {
                ExecutionSettings = chatOrchestrator.CreateExecutionSettings([vesselAgent,bookingAgent])

            };
        }

        public GroupChatOrchestration CreateGroupChat(Kernel kernel, OrchestrationResponseCallback responseCallback)
        {
            
            var vesselAgent = new VesselAgent().Create(kernel);
            var capacityAgent = new CapacityAgent().Create(kernel);
            var bookingAgent = new BookingAgent().Create(kernel);
            GroupChatOrchestration orchestration = new GroupChatOrchestration(
            new ShipmnetGroupManager(kernel.GetRequiredService<IChatCompletionService>()) { MaximumInvocationCount = 3 }, vesselAgent, capacityAgent , bookingAgent)
            {
                ResponseCallback = responseCallback,
            };
            return orchestration;

        }
        
    }


}
