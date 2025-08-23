
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Threading;
using System.Threading.Tasks;

public interface IApprovalService
{
    Task<ChatMessageContent> WaitForApprovalAsync(CancellationToken ct = default);
}
