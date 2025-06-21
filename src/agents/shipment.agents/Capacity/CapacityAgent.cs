using Azure.AI.Agents.Persistent;
using Azure.AI.Projects;
using Azure.Identity;

using infrastructure;

using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.AzureAI;

using ModelContextProtocol.Client;
using System.Diagnostics.CodeAnalysis;
namespace shipment.agents.Capacity
{
    [Experimental("SKEXP0110")]
    public class CapacityAgent(IMCPClientFactory clientFactory,IConfiguration configuration): IAgent
    {      
        public Agent CreateAgents(Kernel kernel)
        {
            var capacityClient = clientFactory.CreateCapacityClient().GetAwaiter().GetResult();
            var capacityTools = capacityClient.ListToolsAsync().GetAwaiter().GetResult();


            AIProjectClient projectClient = new(new Uri(configuration["AgentProjectEndpoint"]), new DefaultAzureCredential());
            PersistentAgentsClient agentsClient = projectClient.GetPersistentAgentsClient();
            PersistentAgent definition = agentsClient.Administration.GetAgent("asst_QWOr2oHmxiwcsguWbiUO1szR");
            AzureAIAgent agent = new(definition, agentsClient)
            {
                Kernel = kernel,
                Arguments = new KernelArguments(new PromptExecutionSettings() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: new() { RetainArgumentTypes = true }) }),
            };
            agent.Kernel.Plugins.AddFromFunctions("CapacityContainerTool", capacityTools.Select(_ => _.AsKernelFunction()));
            return agent;
        }
    }
}
