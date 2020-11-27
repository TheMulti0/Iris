using System.Linq;
using DashboardBackend.Models;
using UpdatesConsumer;

namespace DashboardBackend.Data
{
    public class UpdatesRepository : IUpdatesRepository
    {
        private ApplicationDbContext _dbContext;

        public UpdatesRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Update> Get(PageSearchParams searchParams)
        {
            return _dbContext.Updates
                .Skip(searchParams.PageSize * (searchParams.PageIndex - 1))
                .Take(searchParams.PageSize);
        }
    }
}