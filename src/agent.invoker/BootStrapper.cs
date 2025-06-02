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
        public async Task Run()
        {
           
           await StartGroupChat();
            
        }
        public ValueTask responseCallback(ChatMessageContent response)
        {
            Console.WriteLine(response.Content);
            return ValueTask.CompletedTask;
        }
        public async Task StartGroupChat()
        {
            Console.WriteLine("give the command");
            string query = Console.ReadLine();
            InProcessRuntime runtime = new InProcessRuntime();
            await runtime.StartAsync();
            GroupAgent groupAgent = new GroupAgent();
            var orchestration = groupAgent.CreateAgentGroupChat(_kernel, responseCallback);
            OrchestrationResult<string> result = await orchestration.InvokeAsync(query, runtime);
            await result.GetValueAsync();
            await runtime.RunUntilIdleAsync();
            Console.WriteLine("**************************Good bye from agents**************************");
            Console.ReadLine();
        }

    }
}
