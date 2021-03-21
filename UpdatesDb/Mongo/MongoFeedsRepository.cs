using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDbGenericRepository;

namespace UpdatesDb
{
    public class MongoFeedsRepository : IFeedsRepository
    {
        private readonly IMongoCollection<FeedEntity> _collection;

        public MongoFeedsRepository(
            IMongoDbContext context)
        {
            _collection = context.GetCollection<FeedEntity>();
        }

        public IQueryable<FeedEntity> Get()
        {
            return _collection.AsQueryable();
        }

        public Task<FeedEntity> GetAsync(ObjectId id)
        {
            return _collection
                .AsQueryable()
                .Where(update => update.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task AddOrUpdateAsync(FeedEntity entity)
        {
            FeedEntity existing = await GetAsync(entity.Id);

            if (existing == null)
            {
                await _collection.InsertOneAsync(entity);
                return;
            }

            await _collection.UpdateOneAsync(
                FilterDefinition<FeedEntity>.Empty,
                Builders<FeedEntity>.Update.Set(u => u, entity));
        }
    }
}