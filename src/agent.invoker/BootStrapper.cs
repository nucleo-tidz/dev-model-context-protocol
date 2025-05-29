namespace client
{
    using Microsoft.SemanticKernel;
    using Microsoft.SemanticKernel.Agents.Orchestration;
    using Microsoft.SemanticKernel.Agents.Orchestration;
    using Microsoft.SemanticKernel.Agents.Orchestration.GroupChat;
    using Microsoft.SemanticKernel.Agents.Orchestration.GroupChat;
    using Microsoft.SemanticKernel.Agents.Runtime.InProcess;
    using Microsoft.SemanticKernel.ChatCompletion;
    using shipment.agents.Group;
    using shipment.agents.Orchestrator;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using static System.Net.Mime.MediaTypeNames;

    [Experimental("SKEXP0110")]
    public class BootStrapper : IBootStrapper
    {
        private readonly Kernel _kernel;
        public BootStrapper(Kernel kernel)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
        }
        public async Task Run(bool useOrchestrator = false)
        {
            if (!useOrchestrator)
            {
                await StartGroupChat();
            }
            else
            {
                await StartGroupChatWithOrchestrator();
            }
        }
        public ValueTask responseCallback(ChatMessageContent response)
        {
            Console.WriteLine(response.Content);
            return ValueTask.CompletedTask;
        }
        public async Task StartGroupChatWithOrchestrator()
        {

            InProcessRuntime runtime = new InProcessRuntime();
            await runtime.StartAsync();
            GroupAgent groupAgent = new GroupAgent();
            var orchestration = groupAgent.CreateGroupChat(_kernel, responseCallback);
            OrchestrationResult<string> result = await orchestration.InvokeAsync("Create a booking for 20RY container from New Delhi to Chennai ", runtime);
            await result.GetValueAsync();
            await runtime.RunUntilIdleAsync();
        }
        public async Task StartGroupChat()
        {
            while (true)
            {
                GroupAgent groupAgent = new GroupAgent();
                var agentGroupChat = groupAgent.CreateAgentGroupChat(_kernel);

                Console.WriteLine("give the command");
                string query = Console.ReadLine();
                agentGroupChat.AddChatMessage(new ChatMessageContent(AuthorRole.User, query));
                await foreach (var content in agentGroupChat.InvokeAsync())
                {
                    if (!string.IsNullOrWhiteSpace(content.Content))
                    {
                        Console.WriteLine($"# {content.Role} - {content.AuthorName ?? "*"}: '{content.Content}'");
                    }
                    //Task.Delay(15000).Wait();
                }
            }
        }
    }
}
