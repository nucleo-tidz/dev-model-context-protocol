namespace shipment.agents.Group
{
    using Microsoft.Agents.AI;
    using Microsoft.Agents.AI.Workflows;

    public class GroupAgent(IEnumerable<IAgent> agents) : IGroupAgent
    {   
        public void Create()
        {
            var shipmentAgents = agents.Select(agent => agent.Create()).ToList();
            var workflow = AgentWorkflowBuilder.CreateGroupChatBuilderWith(agents =>
             new RoundRobinGroupChatManager(agents)
             {
                 MaximumIterationCount = 5  
             })
    .     AddParticipants(shipmentAgents.AsEnumerable<AIAgent>)
    .     Build();
        }
    }
}
