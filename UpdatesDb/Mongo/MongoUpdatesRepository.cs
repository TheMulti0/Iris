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
    public class MongoUpdatesRepository : IUpdatesRepository
    {
        private readonly IMongoCollection<UpdateEntity> _collection;

        public MongoUpdatesRepository(
            IMongoDbContext context,
            MongoDbConfig config)
        {
            _collection = context.GetCollection<UpdateEntity>();
            
            if (_collection.Indexes.List().ToList().Count < 2) // There shouldn't be more than two indices in the collection
            {
                CreateExpirationIndex(config);
            }
        }
        
        internal void CreateExpirationIndex(MongoDbConfig config)
        {
            IndexKeysDefinition<UpdateEntity> keys = Builders<UpdateEntity>.IndexKeys
                .Ascending(update => update.SaveDate);

            var options = new CreateIndexOptions
            {
                ExpireAfter = config.UpdatesExpiration ?? TimeSpan.FromDays(7)
            };
            var indexModel = new CreateIndexModel<UpdateEntity>(keys, options);

            _collection.Indexes.CreateOne(indexModel);
        }

        public Paged<UpdateEntity> Get(int pageIndex, int pageSize)
        {
            IMongoQueryable<UpdateEntity> entities = _collection.AsQueryable();
            
            int totalElements = entities.Count();
            int totalPages = RoundZeroDown(totalElements, pageSize);
            int itemIndex = pageIndex * pageSize;
            
            IEnumerable<UpdateEntity> content = GetContent(entities, itemIndex, pageSize, totalElements);

            return new Paged<UpdateEntity>(
                content,
                pageIndex,
                totalPages,
                entities.Count()
            );
        }

        private static IEnumerable<UpdateEntity> GetContent(
            IMongoQueryable<UpdateEntity> entities,
            int itemIndex,
            int pageSize, 
            int totalElements)
        {
            if (itemIndex >= totalElements)
            {
                return new List<UpdateEntity>();
            }

            return entities
                .Skip(itemIndex)
                .Take(pageSize);
        }

        private static int RoundZeroDown(int numerator, int denominator)
        {
            return (numerator + denominator - 1) / denominator;
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
            UpdateEntity existing = await GetAsync(entity.Id);

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