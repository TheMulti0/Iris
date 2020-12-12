using System.Collections.Generic;
using System.Threading.Tasks;
using Common;

namespace UpdatesProducer
{
    public interface IUpdatesProvider
    {
        public Task<IEnumerable<Update>> GetUpdatesAsync(string userId);
    }
}