using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

using System;
using System.Threading;
using System.Threading.Tasks;

public sealed class ApprovalService : IApprovalService
{
    private readonly ISpeechService _speech;

    public ApprovalService(ISpeechService speech)
    {
        _speech = speech ?? throw new ArgumentNullException(nameof(speech));
    }

    public async Task<ChatMessageContent> WaitForApprovalAsync(CancellationToken ct = default)
    {
        await _speech.PauseAsync(ct);
        await _speech.SpeakAsync("Please say approve or reject.", ct);
        var text = await _speech.RecognizeOneUtteranceAsync(ct) ?? string.Empty;
        await _speech.ResumeAsync(ct);

        return new ChatMessageContent(AuthorRole.User, text);
    }
}
