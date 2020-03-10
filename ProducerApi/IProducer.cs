using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProducerApi
{
    public interface IProducer
    {
        Task<IEnumerable<IUpdate>> GetUpdates(long authorId);
    }
}
