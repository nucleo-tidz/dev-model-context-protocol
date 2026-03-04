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

        private string FormatAgentList()
        {
            return string.Join(Environment.NewLine, aIAgents.Select(agent => $"Agent Name - {agent.Name} - Agent Description {agent.Description}"));
        }

        private const string VesselAgentName = nameof(VesselAgent);
        private const string CapacityAgentName = nameof(CapacityAgent);
        private const string BookingAgentName = nameof(BookingAgent);
        public record TerminationResponse(string reason, bool shouldTerminate);
        public record SelectionResponse(string agentName, string reason);
        public static string AgentTermination = $"""
            You are an Agent Terminator, responsible for deciding whether the workflow should be terminated based on the container booking context.
            Use the chat history to assess the current state and follow these rules to make your decision:
            
            **TERMINATION CONDITIONS - ALL must be met to terminate:**
            1. Vessel MUST have been found by {VesselAgentName} (look for VesselId in history)
            2. Capacity MUST have been checked by {CapacityAgentName} (look for {CapacityAgentName} in AuthorName and capacity information like "TEU")
            3. Vessel space MUST have been locked (look for "LockVesselSpace" or "locked" in history from {VesselAgentName})
            4. Booking MUST have been created by {BookingAgentName} (look for "Booking ID" or "BE-" in history from {BookingAgentName})
            
            **ONLY TERMINATE IF:**
            - ALL 4 steps above are completed AND booking was successful, OR
            - Vessel was not found (null/empty) after {VesselAgentName} ran, OR
            - Capacity is 0 TEU after {CapacityAgentName} ran
            
            **DO NOT TERMINATE IF:**
            - Booking has not been created yet (even if vessel is found, capacity checked, and space locked)
            - {BookingAgentName} has not run yet
            - Workflow is still in progress
            
            Carefully analyze the chat history. Look at the AuthorName field to see which agents have run.
            To terminate, respond with true along with your reason.
            Your response should be in json format with property "shouldTerminate": true/false and "reason": "detailed reason for your decision".
            """;
        public static string AgentSelection(string participants) =>
                $"""
                You are an Agent Selector, responsible for choosing the most appropriate agent to handle the next step in a container booking workflow. 
                Use the chat history to understand the current state and make an informed decision.
                
                **STRICT WORKFLOW ORDER - FOLLOW THESE STEPS SEQUENTIALLY:**
                1. {VesselAgentName} - Find a vessel between origin and destination (MUST BE FIRST)
                2. {CapacityAgentName} - Check vessel capacity (MUST BE AFTER vessel is found, BEFORE locking space)
                3. {VesselAgentName} - Lock vessel space (ONLY AFTER capacity is confirmed)
                4. {BookingAgentName} - Create shipment booking (ONLY AFTER space is locked)

                **IMPORTANT RULES:**
                - Do NOT skip {CapacityAgentName} - capacity check is MANDATORY before locking space
                - {CapacityAgentName} can ONLY run if vessel information (VesselId) exists in chat history
                - Do NOT lock vessel space until capacity has been checked
                - Do NOT create booking until space is locked
                - The {VesselAgentName} will be called TWICE: once to find vessel, once to lock space

                **Current State Analysis:**
                - Check if vessel has been found (look for VesselId in history)
                - Check if capacity has been checked (look for {CapacityAgentName} in AuthorName)
                - Check if space has been locked (look for "LockVesselSpace" or "space locked" in history)
                - Check if booking has been created (look for Booking ID in history)

                Below are the available agents with their descriptions:
                {participants}
                
                Your response should be in json format with property "agentName": name of the agent and "reason": "reason for selection based on workflow step".
                """;

        protected override ValueTask<bool> ShouldTerminateAsync(IReadOnlyList<ChatMessage> history, CancellationToken cancellationToken = default)
        {
            List<ChatMessage> request = [.. history, new ChatMessage(ChatRole.System, AgentTermination)];
            TerminationResponse? response = GetResponse<TerminationResponse>(request, cancellationToken);
            
            return ValueTask.FromResult(response.shouldTerminate);
        }
        protected  override ValueTask<AIAgent> SelectNextAgentAsync(IReadOnlyList<ChatMessage> history, CancellationToken cancellationToken = default)
        {

            List<ChatMessage> request = [.. history, new ChatMessage(ChatRole.System, AgentSelection(FormatAgentList()))];
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
