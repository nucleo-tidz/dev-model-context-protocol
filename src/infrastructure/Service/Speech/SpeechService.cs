
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Logging;

using System;
using System.Threading;
using System.Threading.Tasks;

public sealed class SpeechService : ISpeechService, IAsyncDisposable
{
    private readonly SpeechConfig _speechConfig;
    private readonly Func<AudioConfig> _micFactory;

    private SpeechRecognizer? _recognizer;
    private SpeechSynthesizer? _synth;
    private volatile bool _started;
    private volatile bool _paused;
    ILogger<SpeechService> _logger;

    public event Func<string, Task>? RecognizedAsync;

    public SpeechService(SpeechConfig speechConfig, Func<AudioConfig> micFactory,ILogger<SpeechService> logger)
    {
        _speechConfig = speechConfig ?? throw new ArgumentNullException(nameof(speechConfig));
        _micFactory = micFactory ?? throw new ArgumentNullException(nameof(micFactory));
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken ct = default)
    {
        if (_started) return;
        var audio = _micFactory();
        _recognizer = new SpeechRecognizer(_speechConfig, audio);
        _recognizer.Recognizing += (_, e) =>
        {
            if (!_paused) _logger.LogInformation($"[Partial] {e.Result.Text}");
        };
        _recognizer.Recognized += async (_, e) =>
        {
            if (_paused) return;
            if (e.Result.Reason == ResultReason.RecognizedSpeech && !string.IsNullOrWhiteSpace(e.Result.Text))
            {
                var handler = RecognizedAsync;
                if (handler != null)
                {                    
                    _ = handler.Invoke(e.Result.Text);
                }
                _logger.LogInformation($"[Final] {e.Result.Text}");
            }
        };

        await _recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);
        _started = true;
        _synth ??= new SpeechSynthesizer(_speechConfig);
    }

    public async Task StopAsync(CancellationToken ct = default)
    {
        if (!_started) return;
        if (_recognizer != null)
        {
            await _recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
            _recognizer.Dispose();
            _recognizer = null;
        }
        _started = false;
    }

    public Task PauseAsync(CancellationToken ct = default)
    {
        _paused = true;
        return Task.CompletedTask;
    }

    public Task ResumeAsync(CancellationToken ct = default)
    {
        _paused = false;
        return Task.CompletedTask;
    }

    public async Task<string?> RecognizeOneUtteranceAsync(CancellationToken ct = default)
    {     
        using var audio = _micFactory();
        using var oneShot = new SpeechRecognizer(_speechConfig, audio);
        var result = await oneShot.RecognizeOnceAsync().ConfigureAwait(false);

        return result.Reason == ResultReason.RecognizedSpeech && !string.IsNullOrWhiteSpace(result.Text)
            ? result.Text
            : null;
    }

    public async Task SpeakAsync(string text, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        var wasPaused = _paused;
        _paused = true;
        _synth ??= new SpeechSynthesizer(_speechConfig);
        await _synth.SpeakTextAsync(text).ConfigureAwait(false);
         _paused = wasPaused;
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync();
        _synth?.Dispose();
        GC.SuppressFinalize(this);
    }
}
