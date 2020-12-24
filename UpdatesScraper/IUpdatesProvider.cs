using System.Collections.Generic;
using System.Threading.Tasks;
using Common;

namespace UpdatesScraper
{
    public interface IUpdatesProvider
    {
        public Task<IEnumerable<Update>> GetUpdatesAsync(string userId);
    }
}