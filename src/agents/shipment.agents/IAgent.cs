using Microsoft.SemanticKernel.Agents;

namespace shipment.agents
{
    public interface IAgent
    {
        Task<Agent> Create();
    }
}
