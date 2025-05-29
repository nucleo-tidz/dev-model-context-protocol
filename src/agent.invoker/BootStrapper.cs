namespace client
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    using shipment.agents.Group;

    using Microsoft.SemanticKernel;
    using Microsoft.SemanticKernel.ChatCompletion;

    [Experimental("SKEXP0110")]
    public class BootStrapper : IBootStrapper
    {
        private readonly Kernel _kernel;
        public BootStrapper(Kernel kernel)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
        }

#if false
        public async Task Run()
        {
            var emailAgent = new EmailAgent(_kernel).Create();
            ChatHistory chatHistory = new ChatHistory();
            while (true)
            {
                string query = Console.ReadLine();
                chatHistory.Add(new Microsoft.SemanticKernel.ChatMessageContent { Role = AuthorRole.User, Content = query });
                await foreach (var message in emailAgent.InvokeAsync(chatHistory))
                {
                     
                }
            }
        }
#endif
        public async Task Run()
        {
            GroupAgent groupAgent = new GroupAgent();
            var agentGroupChat = groupAgent.CreateAgentGroupChat(_kernel);

            while (true)
            {
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
