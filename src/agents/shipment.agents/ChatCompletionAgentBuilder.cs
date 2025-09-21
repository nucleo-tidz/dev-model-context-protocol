namespace shipment.agents
{
    using Microsoft.SemanticKernel;
    using Microsoft.SemanticKernel.Agents;
    using Microsoft.SemanticKernel.ChatCompletion;

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
        private readonly List<(string pluginName, IEnumerable<KernelFunction> functions)> _mcpPlugins = new();

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

        public ChatCompletionAgentBuilder WithMCPPlugin(string pluginName, IEnumerable<KernelFunction> functions)
        {
            _mcpPlugins.Add((pluginName, functions));
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
            if (_mcpPlugins is not null && _mcpPlugins.Any())
            {
                foreach (var (pluginName, functions) in _mcpPlugins)
                {
                    agentKernel.Plugins.AddFromFunctions(pluginName, functions);
                }
            }

#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            return new ChatCompletionAgent
            {
                Name = _name,
                Instructions = _instructions,
                Kernel = agentKernel,
                Description = _description,
                Arguments = _arguments,
                Template = _template,
                HistoryReducer = _historyReducer
            };
#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        }
    }
}
