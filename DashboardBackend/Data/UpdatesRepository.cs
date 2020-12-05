using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using DashboardBackend.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DashboardBackend.Data
{
    public class UpdatesRepository : IUpdatesRepository
    {
        private readonly IMongoCollection<UpdateEntity> _collection;

        public UpdatesRepository(ApplicationDbContext context)
        {
            _collection = context.Updates;
        }

        public Task<int> CountAsync()
        {
            return _collection.AsQueryable().CountAsync();
        }

        public Task<List<UpdateEntity>> Get(PageSearchParams searchParams)
        {
            IMongoQueryable<UpdateEntity> updates = _collection.AsQueryable()
                .Skip(searchParams.PageSize * searchParams.PageIndex)
                .Take(searchParams.PageSize);
            
            return updates.ToListAsync();
        }

        public async Task AddAsync(UpdateEntity update)
        {
            await _collection.InsertOneAsync(update);
        }

        public async Task DeleteAsync(Guid id)
        {
            UpdateEntity update = await _collection.FindOneAndDeleteAsync(u => u.Id == id);
            
            if (update.Id == id)
            {
                return;
            }
            
            throw new InvalidOperationException();
        }
    }
}