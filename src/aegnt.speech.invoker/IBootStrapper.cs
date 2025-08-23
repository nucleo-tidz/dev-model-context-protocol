namespace client
{
    using System.Threading.Tasks;

    public interface IBootStrapper
    {
        Task StartGroupChatAsync(CancellationToken cancellationToken);
    }
}
