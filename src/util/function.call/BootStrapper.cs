#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable IDE0090 // Use 'new(...)'
#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
namespace sse.client
{
    using Microsoft.SemanticKernel;
    using Microsoft.SemanticKernel.ChatCompletion;
    using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
    using ModelContextProtocol.Client;

    public class BootStrapper : IBootStrapper
    {
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatCompletionService;
        public BootStrapper(Kernel kernel)
        {
            this._kernel = kernel;
            this._chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        }
        public async Task Run()
        {
            //    #region Function Call
            //    _kernel.ImportPluginFromType<RepositionPlugin>();
            //    #endregion

            #region MCP
            var client = await CreateContainerClient();
            var clientTools = client.ListToolsAsync().GetAwaiter().GetResult();
            _kernel.Plugins.AddFromFunctions("ClientTools", clientTools.Select(_ => _.AsKernelFunction()));
            #endregion

            ChatHistory chatHistory = new ChatHistory();
            chatHistory.Add(new Microsoft.SemanticKernel.ChatMessageContent
            {
                Role = AuthorRole.System,
                Content = "You are a container shipment agent of a shipment company, your role is answer user query regarding container repositioning plan , DO NOT assume answer if you dont know the context or you dont have the enough data ",
            });
            Console.WriteLine("Ask Me");
            while (true)
            {
                string query = Console.ReadLine();
                chatHistory.Add(new Microsoft.SemanticKernel.ChatMessageContent { Role = AuthorRole.User, Content = query });


                ChatMessageContent chatMessageContent = await _chatCompletionService.GetChatMessageContentAsync(chatHistory, new AzureOpenAIPromptExecutionSettings()
                {

                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
                }, _kernel);
                Console.WriteLine(chatMessageContent.Content);
                chatHistory.Add(new Microsoft.SemanticKernel.ChatMessageContent { Role = AuthorRole.Assistant, Content = chatMessageContent.Content });
            }

        }

        public async Task<IMcpClient> CreateContainerClient()
        {
            var clientTransport = new SseClientTransport(
                     new SseClientTransportOptions
                     {
                         TransportMode = HttpTransportMode.StreamableHttp,
                         Endpoint = new Uri("https://localhost:7156")
                     }
                 );

            return await McpClientFactory.CreateAsync(clientTransport);
        }
    }

}
#pragma warning restore SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore IDE0090 // Use 'new(...)'
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.