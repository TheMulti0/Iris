using System.Linq;
using DashboardBackend.Models;
using UpdatesConsumer;

namespace DashboardBackend.Data
{
    public interface IUpdatesRepository
    {
        IQueryable<Update> Get(PageSearchParams searchParams);
    }
}