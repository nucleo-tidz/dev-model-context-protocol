using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Protocol;
using shipment.agents.Capacity;
using shipment.agents.Vessel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace shipment.agents.Orchestrator
{

    public class ShipmnetGroupManager : RoundRobinGroupChatManager
    {
        IChatClient _chatClient;
        IReadOnlyList<AIAgent> aIAgents;
        public ShipmnetGroupManager(IReadOnlyList<AIAgent> agents,IChatClient chatClient)
        : base(agents)
        {
            _chatClient = chatClient;
            aIAgents = agents;
        }


        private const string VesselAgentName = nameof(VesselAgent);
        private const string CapacityAgentName = nameof(CapacityAgent);
        private const string BookingAgentName = nameof(BookingAgent);
        public record TerminationResponse(string reason, bool shouldTerminate);
        public record SelectionResponse(string agentName, string reason);
        public static string AgentTermination = $"""
            You are an Agent Terminator, responsible for deciding whether an active agent should be terminated based on the container booking context.
            Use the chat history to assess the current state and follow these rules to make your decision:
            -Only apply termination logic if the agent has already been executed use AuthorName property of chat history to find which agent has run.
              do not evaluate whether vessel information exists unless the {VesselAgentName} has already run ,.
              do not evaluate whether capacity  exists unless the {CapacityAgentName} has already run .
            - Terminate the agent If the vessel information is missing and {VesselAgentName} has already run , DO NOT assume that vessel information is wrong or fictious if a vessel id is present 
            - Terminate the agent if the vessel remaining capacity is 0 TEU and  {CapacityAgentName} has already run., if remaining capacity is more than 0 TEU like 100 TEU DO NOT Terminate 
            - Terminate if booking is created by {BookingAgentName} and Bookind Id is generated
             Use the chat history to understand the current state and make an informed decision ,To terminate the agent respond  true along with your reason to terminate 
            """;
        public static string AgentSelection(string participants) =>
                $"""
                You are an Agent Selector, responsible for choosing the most appropriate agent to handle the next step in a container booking workflow. Use the chat history to understand the current state and make an informed decision.
                Avoid unnecessary agent selection — for example, do not select the Capacity  Agent if vessel capacity has already been confirmed. The required steps in a typical container booking process are:
                - Find a Vessel
                -  Check Vessel Capacity
                - Create Shipment Booking on Vessel

                Your task is to choose the next best agent to continue the process based on what's already been completed.

                Below are the available agents with their descriptions:
                {participants}
                Please respond with only  name of the Agent along with your reason to select the agent.
                """;

        protected override ValueTask<bool> ShouldTerminateAsync(IReadOnlyList<ChatMessage> history, CancellationToken cancellationToken = default)
        {
            List<ChatMessage> request = [.. history, new ChatMessage(ChatRole.System, AgentTermination)];
            TerminationResponse? response = GetResponse<TerminationResponse>(request, cancellationToken);
            return ValueTask.FromResult(response.shouldTerminate);
        }
        protected internal override ValueTask<AIAgent> SelectNextAgentAsync(IReadOnlyList<ChatMessage> history, CancellationToken cancellationToken = default)
        {
            
            List<ChatMessage> request = [.. history, new ChatMessage(ChatRole.System, AgentSelection(""))];
            SelectionResponse? response = GetResponse<SelectionResponse>(request, cancellationToken);
            if(response.agentName == VesselAgentName)
            {
                return ValueTask.FromResult(aIAgents.First(a => a.Name == VesselAgentName));
            }
            else if(response.agentName == CapacityAgentName)
            {
                return ValueTask.FromResult(aIAgents.First(a => a.Name == CapacityAgentName));
            }
            else if(response.agentName == BookingAgentName)
            {
                return ValueTask.FromResult(aIAgents.First(a => a.Name == BookingAgentName));
            }
            else
            {
                throw new InvalidOperationException($"Invalid agent name: {response.agentName}");
            }

        }
        private T GetResponse<T>(IEnumerable<ChatMessage> history, CancellationToken cancellationToken)
        {
           ChatResponse response= _chatClient.GetResponseAsync(history).GetAwaiter().GetResult();
            return JsonSerializer.Deserialize<T>(response.Text);
        }
    }
}
