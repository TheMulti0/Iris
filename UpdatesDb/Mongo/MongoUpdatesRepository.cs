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

        public Slice<UpdateEntity> Get(int startIndex, int limit)
        {
            IQueryable<UpdateEntity> entities = _collection
                .AsQueryable()
                .OrderByDescending(entity => entity.SaveDate);
            
            int totalElements = entities.Count();
            
            IEnumerable<UpdateEntity> content = GetContent(
                entities,
                startIndex,
                limit,
                totalElements);

            return new Slice<UpdateEntity>(
                content,
                startIndex,
                limit,
                totalElements
            );
        }

        private static IEnumerable<UpdateEntity> GetContent(
            IQueryable<UpdateEntity> entities,
            int startIndex,
            int limit, 
            int totalElements)
        {
            if (startIndex >= totalElements)
            {
                return new List<UpdateEntity>();
            }

            return entities
                .Skip(startIndex)
                .Take(limit + 1); // limit is exclusive
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