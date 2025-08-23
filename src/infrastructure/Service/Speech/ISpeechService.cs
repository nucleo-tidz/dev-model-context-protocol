
using System;
using System.Threading;
using System.Threading.Tasks;

public interface ISpeechService
{

    event Func<string, Task>? RecognizedAsync;
    Task StartAsync(CancellationToken ct = default);
    Task StopAsync(CancellationToken ct = default);
    Task PauseAsync(CancellationToken ct = default);
    Task ResumeAsync(CancellationToken ct = default);
    Task<string?> RecognizeOneUtteranceAsync(CancellationToken ct = default);
    Task SpeakAsync(string text, CancellationToken ct = default);
}
