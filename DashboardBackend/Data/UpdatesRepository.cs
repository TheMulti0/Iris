using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DashboardBackend.Models;
using UpdatesConsumer;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DashboardBackend.Data
{
    public class UpdatesRepository : IUpdatesRepository
    {
        private ApplicationDbContext _context;

        public UpdatesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<int> CountAsync()
        {
            return _context.Updates.AsQueryable().CountAsync();
        }

        public Task<List<Update>> Get(PageSearchParams searchParams)
        {
            IMongoQueryable<Update> updates = _context.Updates.AsQueryable()
                .Skip(searchParams.PageSize * searchParams.PageIndex)
                .Take(searchParams.PageSize);
            
            return updates.ToListAsync();
        }

        public async Task AddAsync(Update update)
        {
            await _context.Updates.InsertOneAsync(update);
        }

        public async Task DeleteAsync(Guid id)
        {
            Update update = await _context.Updates.FindOneAndDeleteAsync(u => u.Id == id);
            
            if (update.Id == id)
            {
                return;
            }
            
            throw new InvalidOperationException();
        }
    }
}