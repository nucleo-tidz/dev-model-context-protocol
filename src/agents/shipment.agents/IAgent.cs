using Microsoft.Agents.AI;

namespace shipment.agents
{
    public interface IAgent
    {
        Task<AIAgent> Create();
    }
}
