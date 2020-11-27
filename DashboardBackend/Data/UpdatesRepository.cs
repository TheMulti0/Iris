using System;
using System.Linq;
using System.Threading.Tasks;
using DashboardBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

        public async Task DeleteAsync(long id)
        {
            Update update = await _dbContext.Updates.FindAsync(id);
            EntityEntry<Update> deleted = _dbContext.Updates.Remove(update);
            
            if (deleted.Entity.Id == id)
            {
                await _dbContext.SaveChangesAsync();
                return;
            }
            
            deleted.State = EntityState.Unchanged;
            throw new InvalidOperationException();
        }
    }
}