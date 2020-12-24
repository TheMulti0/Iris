using System.Threading;
using System.Threading.Tasks;
using Common;

namespace ScrapersDistributor
{
    public interface IPollRequestsConsumer
    {
        Task OnRequestAsync(PollRequest request, CancellationToken token);
    }
}