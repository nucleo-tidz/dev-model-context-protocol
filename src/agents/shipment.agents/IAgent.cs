using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace shipment.agents
{
    public interface IAgent
    {
        Agent CreateAgents(Kernel kernel);
    }
}
