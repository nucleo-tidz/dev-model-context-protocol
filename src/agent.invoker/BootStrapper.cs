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
            string usermessage = "book me a 20DRY container from shanghai to copenhagen";
            await using StreamingRun run = await InProcessExecution.Lockstep.RunStreamingAsync(workflow, usermessage);
            await run.TrySendMessageAsync(new TurnToken(emitEvents: true));

            await foreach (WorkflowEvent evt in run.WatchStreamAsync().ConfigureAwait(false))
            {
                if (evt is RequestInfoEvent info)
                {
                    if (info.Request.TryGetDataAs(out FunctionApprovalRequestContent? approvalRequestContent))
                    {
                        Console.WriteLine("Kindly press Y to approve N to reject");
                        string? userInput = Console.ReadLine();
                        bool isApproved = userInput?.Trim().ToUpper() == "Y";
                        await run.SendResponseAsync(info.Request.CreateResponse(approvalRequestContent.CreateResponse(approved: isApproved)));
                    }

                }
                
                //else if (evt is AgentResponseUpdateEvent update)
                //{

                //    AgentResponse response = update.AsResponse();
                //    foreach (ChatMessage message in response.Messages)
                //    {
                //        //Console.WriteLine($"[{update.ExecutorId}]: {message.Text}");
                //    }
                //}
                else if (evt is WorkflowOutputEvent output)
                {
                    var conversationHistory = output.As<List<ChatMessage>>();
                    if (conversationHistory is null)
                        continue;
                    Console.WriteLine("\n=== Final Conversation ===");
                    foreach (var message in conversationHistory)
                    {
                        Console.WriteLine($"{message.AuthorName}: {message.Text}");
                    }
                    Console.ReadLine();
                    break;

                }
            }
        }
    }
}
