namespace shipment.agents
{
    using Microsoft.SemanticKernel;
    using Microsoft.SemanticKernel.Agents;
    using Microsoft.SemanticKernel.ChatCompletion;

    using ModelContextProtocol.Client;

    public class ChatCompletionAgentBuilder
    {
        private Kernel _kernel;
        private string _name = string.Empty;
        private string _instructions = string.Empty;
        private string _description = string.Empty;
        private IPromptTemplate _template;
        private IChatHistoryReducer _historyReducer;
        private KernelArguments _arguments;
        private readonly List<Action<Kernel>> _kernelConfigs = new();
        private List<(string Name, List<McpClientTool> Tools)> _mcpClientTools = new();

        public ChatCompletionAgentBuilder WithName(string name)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            return this;
        }

        public ChatCompletionAgentBuilder WithInstructions(string instructions)
        {
            _instructions = instructions ?? string.Empty;
            return this;
        }

        public ChatCompletionAgentBuilder WithDescription(string description)
        {
            _description = description ?? string.Empty;
            return this;
        }
        public ChatCompletionAgentBuilder WithTemplate(IPromptTemplate template)
        {
            _template = template;
            return this;
        }

        public ChatCompletionAgentBuilder WithHistoryReducer(IChatHistoryReducer reducer)
        {
            _historyReducer = reducer;
            return this;
        }


        public ChatCompletionAgentBuilder WithKernel(Kernel kernel)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            return this;
        }
        public ChatCompletionAgentBuilder WithTool<T>(string? pluginName = null)
        {
            _kernelConfigs.Add(k => k.ImportPluginFromType<T>(pluginName));
            return this;
        }

        public ChatCompletionAgentBuilder WithMCPPlugin(string pluginName, IEnumerable<McpClientTool> mcpTools)
        {
            _mcpClientTools.Add((Name: pluginName, Tools: mcpTools.ToList()));
            return this;
        }
        public ChatCompletionAgentBuilder WithArgumnets(KernelArguments arg)
        {
            _arguments = arg;
            return this;
        }
        public ChatCompletionAgent Build()
        {

            var agentKernel = _kernel.Clone();
            if (_kernelConfigs != null && _kernelConfigs.Any())
            {
                foreach (var cfg in _kernelConfigs)
                {
                    cfg(agentKernel);
                }
            }
            if (_mcpClientTools is not null && _mcpClientTools.Any())
            {
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                foreach (var (Name, Tools) in _mcpClientTools)
                    agentKernel.Plugins.AddFromFunctions(Name, Tools.Select(_ => _.AsKernelFunction()));             
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            }

#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            return new ChatCompletionAgent
            {
                Name = _name,
                Instructions = _instructions,
                Kernel = agentKernel,
                Description = _description,
                Arguments = _arguments,     
            };
#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        }
    }
}
