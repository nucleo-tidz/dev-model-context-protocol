namespace shipment.agents.Group
{
    using Microsoft.Agents.AI;
    using Microsoft.Agents.AI.Workflows;

    public class GroupAgent(IEnumerable<IAgent> agents) : IGroupAgent
    {   
        public async Task<Workflow> Create()
        {
            var shipmentAgents = agents.Select(agent => agent.Create()).ToArray();
            var createdAgents = await Task.WhenAll(shipmentAgents);
            return AgentWorkflowBuilder.CreateGroupChatBuilderWith(agents =>
                new RoundRobinGroupChatManager(agents)
                {
                    MaximumIterationCount = 4 
                })
                .AddParticipants(createdAgents)
                .Build();
        }
    }
}
