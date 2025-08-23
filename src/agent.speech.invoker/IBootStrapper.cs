namespace agent.speech.invoker
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal interface IBootStrapper
    {
        Task StartGroupChatAsync(CancellationToken cancellationToken = default);
    }
}
