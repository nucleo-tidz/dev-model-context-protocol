using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents.Orchestration.GroupChat;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using shipment.agents.Capacity;
using shipment.agents.Vessel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace shipment.agents.Orchestrator
{

    [Experimental("SKEXP0110")]
    public class ShipmnetGroupManager(IChatCompletionService chatCompletion) : GroupChatManager
    {

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
        public override ValueTask<GroupChatManagerResult<string>> FilterResults(ChatHistory history, CancellationToken cancellationToken = default)
        {


            GroupChatManagerResult<string> result = new(history.LastOrDefault()?.Content ?? string.Empty) { Reason = "Default result filter provides the final chat message." };
            return ValueTask.FromResult(result);
        }
        public override ValueTask<GroupChatManagerResult<string>> SelectNextAgent(ChatHistory history, GroupChatTeam team, CancellationToken cancellationToken = default)
        {
            ChatHistory request = [.. history, new ChatMessageContent(AuthorRole.System, AgentSelection(team.FormatList()))];
            SelectionResponse? response = GetResponse<SelectionResponse>(request, cancellationToken);
            Console.WriteLine("\n Orchestrator Selected " + response.agentName + " \n Selection Reason-: " + response.reason + "\n");
            return ValueTask.FromResult(new GroupChatManagerResult<string>(response.agentName) { Reason = response.reason });
        }
        private T GetResponse<T>(ChatHistory request, CancellationToken cancellationToken)
        {
            var result = chatCompletion.GetChatMessageContentsAsync(request, new AzureOpenAIPromptExecutionSettings() { ResponseFormat = typeof(T) }, kernel: null, cancellationToken)
                .GetAwaiter()
                .GetResult();
            return JsonSerializer.Deserialize<T>(result[0].Content.ToString());
        }
        public override ValueTask<GroupChatManagerResult<bool>> ShouldRequestUserInput(ChatHistory history, CancellationToken cancellationToken = default)
        {
            GroupChatManagerResult<bool> result = new(false) { Reason = "The group chat manager does not request user input." };
            return ValueTask.FromResult(result);
        }
        public override ValueTask<GroupChatManagerResult<bool>> ShouldTerminate(ChatHistory history, CancellationToken cancellationToken = default)
        {
            var baseResult = base.ShouldTerminate(history, cancellationToken).Result;
            if (baseResult.Value)
            {
                return ValueTask.FromResult(baseResult);
            }
            ChatHistory request = [.. history, new ChatMessageContent(AuthorRole.System, AgentTermination)];

            TerminationResponse? response = GetResponse<TerminationResponse>(request, cancellationToken);
            Console.WriteLine("\n Terminated: " + (response.shouldTerminate ? "Yes" : "No") + " ,\n Termination Reason:" + response.reason + "\n");
            return ValueTask.FromResult(new GroupChatManagerResult<bool>(response.shouldTerminate) { Reason = response.reason });
        }
    }
}
