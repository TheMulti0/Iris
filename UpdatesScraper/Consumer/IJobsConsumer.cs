using System.Threading;
using System.Threading.Tasks;
using Common;

namespace UpdatesScraper
{
    public interface IJobsConsumer
    {
        Task OnJobAsync(User user, CancellationToken token);
    }
}