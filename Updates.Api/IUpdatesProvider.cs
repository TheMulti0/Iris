using System.Collections.Generic;
using System.Threading.Tasks;

namespace Updates.Api
{
    public interface IUpdatesProvider
    {
        Task<IEnumerable<Update>> GetUpdates(long authorId);
    }
}
