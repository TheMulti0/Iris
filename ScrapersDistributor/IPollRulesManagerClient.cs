using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace ScrapersDistributor
{
    public interface IPollRulesManagerClient
    {
        Task<List<UserPollRule>> Get(CancellationToken token);
    }
}