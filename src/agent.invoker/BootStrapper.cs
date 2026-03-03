namespace client
{
    using Microsoft.Agents.AI;
    using Microsoft.Agents.AI.Workflows;
    using Microsoft.Extensions.AI;
    using shipment.agents.Group;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;


    public class BootStrapper(IGroupAgent groupAgent) : IBootStrapper
    {
        public async Task Run()
        {
            var workflow = await groupAgent.Create();
            string usermessage = Console.ReadLine();
            await using StreamingRun run = await InProcessExecution.Lockstep.RunStreamingAsync(workflow, usermessage);
            await run.TrySendMessageAsync(new TurnToken(emitEvents: true));

            await foreach (WorkflowEvent evt in run.WatchStreamAsync().ConfigureAwait(false))
            {
                if (evt is AgentResponseUpdateEvent update)
                {
                    // Process streaming agent responses
                    AgentResponse response = update.AsResponse();
                    foreach (ChatMessage message in response.Messages)
                    {
                        Console.WriteLine($"[{update.ExecutorId}]: {message.Text}");
                    }
                }
                else if (evt is WorkflowOutputEvent output)
                {
                    // Workflow completed
                    var conversationHistory = output.As<List<ChatMessage>>();
                    Console.WriteLine("\n=== Final Conversation ===");
                    foreach (var message in conversationHistory)
                    {
                        Console.WriteLine($"{message.AuthorName}: {message.Text}");
                    }
                    break;
                }
            }
        }
    }
}
