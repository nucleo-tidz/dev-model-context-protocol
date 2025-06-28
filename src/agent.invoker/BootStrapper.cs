namespace client
{
    using Microsoft.SemanticKernel;
    using Microsoft.SemanticKernel.Agents.Orchestration;
    using Microsoft.SemanticKernel.Agents.Runtime.InProcess;
    using Microsoft.SemanticKernel.ChatCompletion;
    using shipment.agents.Group;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;

    [Experimental("SKEXP0110")]
    public class BootStrapper : IBootStrapper
    {
        private readonly Kernel _kernel;
        private readonly IGroupAgent _groupAgent;

        public BootStrapper(Kernel kernel, IGroupAgent groupAgent)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            _groupAgent = groupAgent ?? throw new ArgumentNullException(nameof(groupAgent));
        }

        private static ValueTask ResponseCallback(ChatMessageContent response)
        {
            Console.WriteLine(response.Content);
            return ValueTask.CompletedTask;
        }
        public static ValueTask<ChatMessageContent> InteractiveCallBack()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("===================================");
            Console.WriteLine("Waiting for human to Approve");
            ChatMessageContent input = new(AuthorRole.User, Console.ReadLine());
            Console.WriteLine("===================================");
            Console.ForegroundColor = ConsoleColor.White;
            return ValueTask.FromResult(input);
        }

        public async Task StartGroupChatAsync(CancellationToken cancellationToken = default)
        {
            Console.WriteLine("Enter your command:");
            string? query = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(query))
            {
                Console.WriteLine("No command entered. Exiting.");
                return;
            }

            var runtime = new InProcessRuntime();
            await runtime.StartAsync(cancellationToken);

            var orchestration = await _groupAgent.CreateAgentGroupChat(ResponseCallback, InteractiveCallBack);
            OrchestrationResult<string> result = await orchestration.InvokeAsync(query, runtime, cancellationToken);
            await result.GetValueAsync();
            await runtime.RunUntilIdleAsync();

            Console.WriteLine("**************************Good bye from agents**************************");
            Console.ReadLine();
        }
    }
}
