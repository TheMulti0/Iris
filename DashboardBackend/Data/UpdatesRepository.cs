using System.Linq;
using System.Threading.Tasks;
using DashboardBackend.Models;
using Microsoft.EntityFrameworkCore;
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

        public Task<int> CountAsync()
        {
            return _dbContext.Updates.CountAsync();
        }

        public IQueryable<Update> Get(PageSearchParams searchParams)
        {
            return _dbContext.Updates
                .Skip(searchParams.PageSize * (searchParams.PageIndex - 1))
                .Take(searchParams.PageSize);
        }
    }
}