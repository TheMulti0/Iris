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
    public class MongoUpdatesRepository : IUpdatesRepository
    {
        private readonly IMongoCollection<UpdateEntity> _collection;

        public MongoUpdatesRepository(IMongoDbContext context)
        {
            _collection = context.GetCollection<UpdateEntity>();
        }

        public IQueryable<UpdateEntity> Get()
        {
            return _collection
                .AsQueryable();
        }

        public Task<UpdateEntity> GetAsync(ObjectId id)
        {
            return _collection
                .AsQueryable()
                .Where(update => update.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task AddOrUpdateAsync(UpdateEntity entity)
        {
            var existing = await GetAsync(entity.Id);

            if (existing == null)
            {
                await _collection.InsertOneAsync(entity);
                return;
            }

            await _collection.UpdateOneAsync(
                FilterDefinition<UpdateEntity>.Empty,
                Builders<UpdateEntity>.Update.Set(u => u, entity));
        }
    }
}