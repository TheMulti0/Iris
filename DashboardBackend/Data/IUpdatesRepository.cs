using System.Linq;
using System.Threading.Tasks;
using DashboardBackend.Models;
using UpdatesConsumer;

namespace DashboardBackend.Data
{
    public interface IUpdatesRepository
    {
        Task<int> CountAsync();
        
        IQueryable<Update> Get(PageSearchParams searchParams);
    }
}