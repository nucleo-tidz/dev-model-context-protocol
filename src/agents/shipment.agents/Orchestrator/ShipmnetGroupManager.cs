using Azure.Core;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents.Orchestration.GroupChat;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using shipment.agents.Capacity;
using shipment.agents.Vessel;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace shipment.agents.Orchestrator
{
    [Experimental("SKEXP0110")]
    public class ShipmnetGroupManager(IChatCompletionService chatCompletion) : GroupChatManager
    {
        private const string VesselAgentName = nameof(VesselAgent);
        private const string CapacityAgentName = nameof(CapacityAgent);
        private const string BookingAgentName = nameof(BookingAgent);
        public record TerminationResponse(string reason, bool shouldTerminate);
        public record SelectionResponse(string agentName,string reason);
        public static string AgentTermination =$"""
            You are an Agent Terminator, responsible for deciding whether an active agent should be terminated based on the container booking context.
            Use the chat history to assess the current state and follow these rules to make your decision:
            -Only apply termination logic if the agent has already been executed use AuthorName propert of chat history to find which agent has run.
              do not evaluate whether vessel information exists unless the {VesselAgentName} has already run ,.
              do not evaluate whether capacity  exists unless the {CapacityAgentName} has already run .
            - Terminate the agent If the vessel information is missing and {VesselAgentName} has already run , DO NOT assume that vessel information is wrong or fictious if a vessel id is present 
            - Terminate the agent if the vessel remaining capacity is 0 TEU and  {CapacityAgentName} has already run., if remaining capacity is more than 0 TEU like 100 TEU DO NOT Terminate 

             Use the chat history to understand the current state and make an informed decision ,To terminate the agent respond  true along with your reason to terminate 
            """;

        public static string AgentSelection (string participants) =>
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
            SelectionResponse? response = GetResponsec<SelectionResponse>(request, cancellationToken);
#if false
            string? lastAgent = history.LastOrDefault()?.AuthorName;
            if (string.IsNullOrWhiteSpace(lastAgent))
            {
                return ValueTask.FromResult(new GroupChatManagerResult<string>(VesselAgentName) { Reason = "Selected vessel agent" });
            }
            else if (lastAgent == VesselAgentName)
            {
                return ValueTask.FromResult(new GroupChatManagerResult<string>(CapacityAgentName) { Reason = "Found a vessel ,selected capacity agent" });
            }
            else
            {
                return ValueTask.FromResult(new GroupChatManagerResult<string>(BookingAgentName) { Reason = "Found capacity Selected booking agent" });
            }
#endif
            return ValueTask.FromResult(new GroupChatManagerResult<string>(response.agentName) { Reason = response.reason });

        }

        private T GetResponsec<T>(ChatHistory request, CancellationToken cancellationToken)
        {
            var result = chatCompletion.GetChatMessageContentsAsync(request, new AzureOpenAIPromptExecutionSettings () { ResponseFormat = typeof(T) }, kernel: null, cancellationToken)
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
                
            TerminationResponse? response = GetResponsec<TerminationResponse>(request, cancellationToken);
            return ValueTask.FromResult(new GroupChatManagerResult<bool>(response.shouldTerminate) { Reason = response.reason });
        }
    }
}
