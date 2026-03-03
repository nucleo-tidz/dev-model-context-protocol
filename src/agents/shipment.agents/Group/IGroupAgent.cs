using Microsoft.Agents.AI.Workflows;
using System.Diagnostics.CodeAnalysis;

namespace shipment.agents.Group
{
   
    public interface IGroupAgent
    {
        Task<Workflow> Create();
    }
}
