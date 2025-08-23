namespace client
{
    using Azure;

    using Microsoft.CognitiveServices.Speech;
    using Microsoft.SemanticKernel;
    using Microsoft.SemanticKernel.Agents.Orchestration;
    using Microsoft.SemanticKernel.Agents.Runtime.InProcess;
    using Microsoft.SemanticKernel.ChatCompletion;

    using shipment.agents.Group;

    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;

    using static System.Net.Mime.MediaTypeNames;

    [Experimental("SKEXP0110")]
    public class BootStrapper : IBootStrapper
    {
        private readonly Kernel _kernel;
        private readonly IGroupAgent _groupAgent;

        private readonly SpeechRecognizer _recognizer;
        private readonly SpeechSynthesizer _synthesizer;
        private bool _inApprovalPhase = false;
        private TaskCompletionSource<ChatMessageContent>? taskCompletionSource;

        public BootStrapper(
            Kernel kernel,
            IGroupAgent groupAgent,
            SpeechRecognizer recognizer,
            SpeechSynthesizer synthesizer)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            _groupAgent = groupAgent ?? throw new ArgumentNullException(nameof(groupAgent));
            _recognizer = recognizer ?? throw new ArgumentNullException(nameof(recognizer));
            _synthesizer = synthesizer ?? throw new ArgumentNullException(nameof(synthesizer));
        }

        private ValueTask ResponseCallback(ChatMessageContent response)
        {
            if (!string.IsNullOrEmpty(response.Content))
            {
                return new ValueTask(Speak(response.Content));
            }
            return ValueTask.CompletedTask;
        }

        private async Task Speak(string response)
        {
            Console.WriteLine(response);
            await _recognizer.StopContinuousRecognitionAsync();
            await _synthesizer.SpeakTextAsync(response);
            await _recognizer.StartContinuousRecognitionAsync();
        }


        private ValueTask<ChatMessageContent> InteractiveCallBack()
        {
            Speak("Please approve the plan for me to create the bookking").Wait();
            _inApprovalPhase = true;
            taskCompletionSource = new TaskCompletionSource<ChatMessageContent>();
            return new ValueTask<ChatMessageContent>(taskCompletionSource.Task);
        }

        public async Task StartGroupChatAsync(CancellationToken cancellationToken = default)
        {
            Console.WriteLine("Start speaking...");

            _recognizer.Recognizing += (s, e) =>
            {
                Console.WriteLine($"[Partial] {e.Result.Text}");
            };

            _recognizer.Recognized += async (s, e) =>
            {
                if (e.Result.Reason == ResultReason.RecognizedSpeech &&
                    !string.IsNullOrEmpty(e.Result.Text))
                {
                    Console.WriteLine($"[Final] {e.Result.Text}");

                    if (_inApprovalPhase && taskCompletionSource is not null)
                    {
                        taskCompletionSource.TrySetResult(new ChatMessageContent(AuthorRole.User, e.Result.Text));
                        _inApprovalPhase = false;
                    }
                    else
                    {
                        await CallSemanticKernel(e.Result.Text, cancellationToken);
                    }
                }
            };

            await _recognizer.StartContinuousRecognitionAsync();
            Console.ReadKey();
            await _recognizer.StopContinuousRecognitionAsync();
        }

        private async Task CallSemanticKernel(string query, CancellationToken cancellationToken)
        {
            var runtime = new InProcessRuntime();
            await runtime.StartAsync(cancellationToken);

            var orchestration = await _groupAgent.CreateAgentGroupChat(ResponseCallback, InteractiveCallBack);
            OrchestrationResult<string> result = await orchestration.InvokeAsync(query, runtime, cancellationToken);
            await result.GetValueAsync();
            await runtime.RunUntilIdleAsync();
        }
    }
}
