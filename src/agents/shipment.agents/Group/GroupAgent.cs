namespace shipment.agents.Group
{
    using Microsoft.Agents.AI;
    using Microsoft.Agents.AI.Workflows;
    using Microsoft.Extensions.AI;
    using shipment.agents.Orchestrator;

    public class GroupAgent(IEnumerable<IAgent> agents,IChatClient chatClient) : IGroupAgent
    {   
        public async Task<Workflow> Create()
        {
            var shipmentAgents = agents.Select(agent => agent.Create()).ToArray();
            var createdAgents = await Task.WhenAll(shipmentAgents);
            return AgentWorkflowBuilder.CreateGroupChatBuilderWith(agents =>
                new ShipmnetGroupManager(agents, chatClient)
                {
                    MaximumIterationCount = 4 
                })
                .AddParticipants(createdAgents)
                .Build();
        }
    }
}
