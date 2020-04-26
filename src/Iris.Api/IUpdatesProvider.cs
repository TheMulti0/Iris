using System.Collections.Generic;
using System.Threading.Tasks;

namespace Iris.Api
{
    public interface IUpdatesProvider
    {
        Task<IEnumerable<Update>> GetUpdates(string userName);
    }
}
