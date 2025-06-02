namespace shipment.agents.Orchestrator
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.SemanticKernel;
    using Microsoft.SemanticKernel.Agents;
    using Microsoft.SemanticKernel.Agents.Chat;
    using Microsoft.SemanticKernel.ChatCompletion;
    using shipment.agents.Capacity;
    using shipment.agents.Vessel;
    [Experimental("SKEXP0110")]
    public class Orchestrator(Kernel kernel)
    {
        private const string VesselAgentName = nameof(VesselAgent);
        private const string CapacityAgentName = nameof(CapacityAgent);
        private const string BookingAgentName = nameof(BookingAgent);

        [Experimental("SKEXP0110")]
        public KernelFunctionSelectionStrategy CreateSelectionStrategy()
        {
            var selectionFunction = KernelFunctionFactory.CreateFromPrompt(
                  $$$"""
               Your job is to determine which participant (agent) should take the next turn in the shipment booking process based on the conversation history.

               Choose only from these participants:
               - {{{VesselAgentName}}}
               - {{{CapacityAgentName}}}
               - {{{BookingAgentName}}}


               1. Start with {{{VesselAgentName}}} to find the vessel between an origin and destination city.
               2. If the last message was from {{{VesselAgentName}}} and contain a valid vessel id, Then invoke {{{CapacityAgentName}}} to check the space left on the vessel.
               3. If the last message was from {{{CapacityAgentName}}} and contains a valid space like 100 TEU, invoke {{{BookingAgentName}}} to create the shipment booking.

               History:
               {{$history}}

               Return ONLY  the name of the next agent to act. Do NOT include any explanation or extra text.Just the name.
               """
            );

            return new KernelFunctionSelectionStrategy(selectionFunction, kernel)
            {
                HistoryVariableName = "history",
                HistoryReducer = new ChatHistoryTruncationReducer(1),

            };
        }

        [Experimental("SKEXP0110")]
        public KernelFunctionTerminationStrategy CreateTerminationStrategy(ChatCompletionAgent[] agents)
        {
            var terminateFunction = KernelFunctionFactory.CreateFromPrompt(
                $$$"""
            Determine if the shipment booking is complete based on whether all three agents have completed their tasks:
            - {{{VesselAgentName}}} has found the vessel between origin and destination.
            -  {{{CapacityAgentName}}} has found the space left on the vessel.
            - {{{BookingAgentName}}} has created a shipment booking.

            If all three agents have completed their tasks or vessel agent has returned null, respond with a single word: terminate.
            Otherwise, respond with: continue.

            History:
            {{$history}}
            """
            );

            return new KernelFunctionTerminationStrategy(terminateFunction, kernel)
            {
                Agents = agents,
                ResultParser = result => result.GetValue<string>()?.Contains("terminate", StringComparison.OrdinalIgnoreCase) ?? false,
                HistoryVariableName = "history",
                MaximumIterations = 3,
                HistoryReducer = new ChatHistoryTruncationReducer(1),
            };
        }

        [Experimental("SKEXP0110")]
        public AgentGroupChatSettings CreateExecutionSettings(ChatCompletionAgent[] agents)
        {
            return new AgentGroupChatSettings()
            {
                TerminationStrategy = CreateTerminationStrategy(agents),
                SelectionStrategy = CreateSelectionStrategy(),
                
            };
        }
    }
}
