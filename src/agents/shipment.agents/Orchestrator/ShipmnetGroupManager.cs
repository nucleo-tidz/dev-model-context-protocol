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
        public static string VesselTermination =$"""
             Based on on the history provided check if a valid vessel information like vessel id , name etc is present or not , 
            if the vessel information is present respond false otherwise true 
            """;
        public override ValueTask<GroupChatManagerResult<string>> FilterResults(ChatHistory history, CancellationToken cancellationToken = default)
        {
            GroupChatManagerResult<string> result = new(history.LastOrDefault()?.Content ?? string.Empty) { Reason = "Default result filter provides the final chat message." };
            return ValueTask.FromResult(result);
        }
        public override ValueTask<GroupChatManagerResult<string>> SelectNextAgent(ChatHistory history, GroupChatTeam team, CancellationToken cancellationToken = default)
        {
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
        }

        public override ValueTask<GroupChatManagerResult<bool>> ShouldRequestUserInput(ChatHistory history, CancellationToken cancellationToken = default)
        {
            GroupChatManagerResult<bool> result = new(false) { Reason = "The group chat manager does not request user input." };
            return ValueTask.FromResult(result);
        }
        public override ValueTask<GroupChatManagerResult<bool>> ShouldTerminate(ChatHistory history, CancellationToken cancellationToken = default)
        {
            bool shouldEnd = false;
            string terminationReason=string.Empty;
            var baseResult = base.ShouldTerminate(history, cancellationToken).Result;
            if (baseResult.Value)
            {
                return ValueTask.FromResult(baseResult);
            }
            if (history.Any(_ => _.AuthorName == VesselAgentName))
            {
                ChatHistory request = [.. history, new ChatMessageContent(AuthorRole.System, VesselTermination)];
                AzureOpenAIPromptExecutionSettings executionSettings = new() { ResponseFormat = typeof(TerminationResponse) };
                var result = chatCompletion.GetChatMessageContentsAsync(request, executionSettings, kernel: null, cancellationToken).GetAwaiter().GetResult();
                var response = JsonSerializer.Deserialize<TerminationResponse>(result[0].Content.ToString());
                if (response.shouldTerminate)
                {
                    shouldEnd = true;
                    terminationReason = response.reason;
                }
            }
            return ValueTask.FromResult(new GroupChatManagerResult<bool>(shouldEnd) { Reason = terminationReason });
        }
    }
}
