using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace ScrapersDistributor
{
    public interface ISubscriptionsManagerClient
    {
        Task<List<Subscription>> Get(CancellationToken token);
    }
}