
using System.Diagnostics.CodeAnalysis;
using agent.speech.invoker;

using Azure;

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents.Runtime.InProcess;
using shipment.agents.Group;
[Experimental("SKEXP0110")]
public class BootStrapper : IBootStrapper
{
    private readonly Kernel _kernel;
    private readonly IGroupAgent _groupAgent;
    private readonly ISpeechService _speech;
    private readonly IApprovalService _approval;
    private readonly ILogger<BootStrapper> _logger;

    public BootStrapper(
        Kernel kernel,
        IGroupAgent groupAgent,
        ISpeechService speech,
        IApprovalService approval,
        ILogger<BootStrapper> logger)
    {
        _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
        _groupAgent = groupAgent ?? throw new ArgumentNullException(nameof(groupAgent));
        _speech = speech ?? throw new ArgumentNullException(nameof(speech));
        _approval = approval ?? throw new ArgumentNullException(nameof(approval));
        _logger = logger;
    }
    public async Task StartGroupChatAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Voice chat ready");
        await _speech.SpeakAsync("Hello! I’m your AI assistant for container bookings. Just tell me which type of container you need and the route from where to where  and I’ll take care of it.");
        _speech.RecognizedAsync += async text =>
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                await _speech.SpeakAsync("Please wait while I make the booking");
                await CallSemanticKernel(text, cancellationToken);
            }
        };
        await _speech.StartAsync(cancellationToken);
        _logger.LogInformation("Press any key to exit.");
        Console.ReadKey();
        await _speech.StopAsync(cancellationToken);
    }
    private ValueTask ResponseCallback(ChatMessageContent response)
    {
        if (!string.IsNullOrWhiteSpace(response?.Content))
        {
            return new ValueTask(_speech.SpeakAsync(response.Content));
        }
        return ValueTask.CompletedTask;
    }
    private async ValueTask<ChatMessageContent> InteractiveCallBack()
    {
        return await _approval.WaitForApprovalAsync();
    }
    private async Task CallSemanticKernel(string query, CancellationToken cancellationToken)
    {
        var runtime = new InProcessRuntime();
        await runtime.StartAsync(cancellationToken);
        var orchestration = await _groupAgent.CreateAgentGroupChat(ResponseCallback, InteractiveCallBack);
        var result = await orchestration.InvokeAsync(query, runtime, cancellationToken);
        await result.GetValueAsync();
        await runtime.RunUntilIdleAsync();
    }
}
