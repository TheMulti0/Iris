using System.Threading;
using System.Threading.Tasks;
using Common;

namespace ScrapersDistributor
{
    public interface IPollRequestsConsumer
    {
        Task OnRequestAsync(ChatPollRequest request, CancellationToken token);
    }
}