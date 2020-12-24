using System.Threading;
using System.Threading.Tasks;
using Common;

namespace PollRulesManager
{
    public interface IChatPollRequestsConsumer
    {
        Task OnRequestAsync(ChatPollRequest request, CancellationToken token);
    }
}