namespace client
{
    using Azure;

    using Microsoft.CognitiveServices.Speech;
    using Microsoft.CognitiveServices.Speech.Audio;
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

        SpeechConfig _speechConfig;
        SpeechRecognizer _recognizer;
        SpeechSynthesizer _synthesizer;

        public BootStrapper(Kernel kernel,
        IGroupAgent groupAgent,

        SpeechRecognizer recognizer,
        SpeechSynthesizer synthesizer)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            _groupAgent = groupAgent ?? throw new ArgumentNullException(nameof(groupAgent));

            _recognizer = recognizer;
            _synthesizer = synthesizer;

        }

        private ValueTask ResponseCallback(ChatMessageContent response)
        {
            if (response.Content is not null)
            {
                Speak(response.Content).Wait();
            }
            return ValueTask.CompletedTask;
        }
        public async Task Speak(string response)
        {
            Console.WriteLine(response);
            await _recognizer.StopContinuousRecognitionAsync();
            var result = await _synthesizer.SpeakTextAsync(response);

            await _recognizer.StartContinuousRecognitionAsync();
        }
        public static ValueTask<ChatMessageContent> InteractiveCallBack()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("===================================");
            Console.WriteLine("Waiting for human to Approve");
            ChatMessageContent input = new(AuthorRole.User, Console.ReadLine());
            Console.WriteLine("===================================");
            Console.ForegroundColor = ConsoleColor.White;
            return ValueTask.FromResult(input);
        }

        public async Task StartGroupChatAsync(CancellationToken cancellationToken = default)
        {
            Console.WriteLine("Start");
            _recognizer.Recognizing += (s, e) =>
            {
                Console.WriteLine($"[Partial] {e.Result.Text}");
            };

            _recognizer.Recognized += async (s, e) =>
            {
                if (e.Result.Reason == ResultReason.RecognizedSpeech)
                {
                    if (!string.IsNullOrEmpty(e.Result.Text))
                    {
                        Console.WriteLine($"[Final] {e.Result.Text}");
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

            Console.WriteLine("**************************Good bye from agents**************************");
            Console.ReadLine();
        }
    }
}
