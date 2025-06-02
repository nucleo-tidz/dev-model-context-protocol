#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
namespace shipment.client
{
    using Microsoft.SemanticKernel;
    using Microsoft.SemanticKernel.ChatCompletion;
    using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
    using ModelContextProtocol.Client;
    using System.Threading.Tasks;

    public class BootStrapper : IBootStrapper
    {
        Kernel _kernel;
        private readonly IChatCompletionService _chatCompletionService;
        public BootStrapper(Kernel kernel)
        {
            this._kernel = kernel;
            this._chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        }
        public async Task Run()
        {
            var clientfactory = new ClientFactory();
      

            var vesselClient = await (clientfactory.CreateVesselClient());
            var vesselTools = await vesselClient.ListToolsAsync();
            _kernel.Plugins.AddFromFunctions("VesselContainerTool", vesselTools.Select(_ => _.AsKernelFunction()));


            var bookingClient = await (clientfactory.CreateBookingClient());
            var bookingTools = await bookingClient.ListToolsAsync();
            _kernel.Plugins.AddFromFunctions("BookingTool", bookingTools.Select(_ => _.AsKernelFunction()));

            var capacityClient = await (clientfactory.CreateCapacityClient());
            var capacityTools = await capacityClient.ListToolsAsync();
            _kernel.Plugins.AddFromFunctions("CapacityTool", capacityTools.Select(_ => _.AsKernelFunction()));

            ChatHistory chatHistory = new ChatHistory();
            chatHistory.Add(new Microsoft.SemanticKernel.ChatMessageContent { Role = AuthorRole.System, Content = "You are a container shipment agent of a shipment comapny, your role is answer user query regarding conatainer shipment , vessel , and containers  ", });
            Console.WriteLine("Ask Me");
            while (true)
            {
                string query = Console.ReadLine();
                chatHistory.Add(new Microsoft.SemanticKernel.ChatMessageContent { Role = AuthorRole.User, Content = query });


                ChatMessageContent chatMessageContent = await _chatCompletionService.GetChatMessageContentAsync(chatHistory, new AzureOpenAIPromptExecutionSettings()
                {
                    Temperature = 0,
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: new FunctionChoiceBehaviorOptions()
                    {
                        RetainArgumentTypes = true
                    })
                }, _kernel);
                Console.WriteLine(chatMessageContent.Content);
                chatHistory.Add(new Microsoft.SemanticKernel.ChatMessageContent { Role = AuthorRole.Assistant, Content = chatMessageContent.Content });
            }
        }
    }
}
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed