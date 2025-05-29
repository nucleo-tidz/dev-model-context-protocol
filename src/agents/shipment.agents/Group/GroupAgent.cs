namespace shipment.agents.Group
{
    using System.Diagnostics.CodeAnalysis;
    using agents.Orchestrator;
    using Microsoft.SemanticKernel;
    using Microsoft.SemanticKernel.Agents;
    using shipment.agents.Capacity;
    using shipment.agents.Vessel;

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
    }
}
