using System.Threading;
using System.Threading.Tasks;
using Common;

namespace UpdatesScraper
{
    public interface IJobsConsumer
    {
        Task OnJobAsync(string userId, CancellationToken token);
    }
}