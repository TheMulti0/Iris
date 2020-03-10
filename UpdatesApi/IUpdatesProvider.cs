using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProducerApi
{
    public interface IUpdatesProvider
    {
        Task<IEnumerable<IUpdate>> GetUpdates(long authorId);
    }
}
