namespace client
{
    using System.Threading.Tasks;

    public interface IBootStrapper
    {
        Task Run(bool useOrchestrator=false);
    }
}
