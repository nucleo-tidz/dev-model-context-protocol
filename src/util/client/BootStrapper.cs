#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable IDE0090 // Use 'new(...)'
#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
namespace client
{
    using Microsoft.SemanticKernel;
    using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

    using ModelContextProtocol.Client;

    public class BootStrapper(Kernel kernel) : IBootStrapper
    {

        public async Task Run()
        {
            var clientfactory = new ClientFactory();
            var client = await (clientfactory.Create());

            var tools = await client.ListToolsAsync();


            kernel.Plugins.AddFromFunctions("ShipmentContainerTool", tools.Select(_ => _.AsKernelFunction()));
            var functionChoiceBehaviorOptions = new FunctionChoiceBehaviorOptions();

            functionChoiceBehaviorOptions.RetainArgumentTypes = true;
            AzureOpenAIPromptExecutionSettings azureOpenAIPromptExecutionSettings = new AzureOpenAIPromptExecutionSettings()
            {
                Temperature = 0,
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: functionChoiceBehaviorOptions),
            };

            var result = await kernel.InvokePromptAsync("What is the state of container with ID AHM1345 ?", new(azureOpenAIPromptExecutionSettings));
        }
    }

}
#pragma warning restore SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore IDE0090 // Use 'new(...)'
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.