namespace client
{
    using Microsoft.SemanticKernel;
    using Microsoft.SemanticKernel.Agents.Orchestration;
    using Microsoft.SemanticKernel.Agents.Runtime.InProcess;
    using shipment.agents.Group;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    [Experimental("SKEXP0110")]
    public class BootStrapper : IBootStrapper
    {
        private readonly Kernel _kernel;
        IGroupAgent _groupAgent;
        public BootStrapper(Kernel kernel, IGroupAgent groupAgent)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            _groupAgent = groupAgent;
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
            InProcessRuntime runtime = new();
            await runtime.StartAsync();
           
            var orchestration = _groupAgent.CreateAgentGroupChat(responseCallback);
            OrchestrationResult<string> result = await orchestration.InvokeAsync(query, runtime);
            await result.GetValueAsync();
            await runtime.RunUntilIdleAsync();
            Console.WriteLine("**************************Good bye from agents**************************");
            Console.ReadLine();
        }

    }
}
