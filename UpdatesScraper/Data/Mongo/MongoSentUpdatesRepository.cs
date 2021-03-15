using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDbGenericRepository;

namespace UpdatesScraper
{
    public class MongoSentUpdatesRepository : ISentUpdatesRepository
    {
        private readonly IMongoCollection<SentUpdate> _collection;

        public MongoSentUpdatesRepository(
            IMongoDbContext context,
            MongoDbConfig config)
        {
            _collection = context.GetCollection<SentUpdate>();

            if (_collection.Indexes.List().ToList().Count < 2) // There shouldn't be more than two indices in the collection
            {
                CreateExpirationIndex(config);
            }
        }

        internal void CreateExpirationIndex(MongoDbConfig config)
        {
            IndexKeysDefinition<SentUpdate> keys = Builders<SentUpdate>.IndexKeys
                .Ascending(update => update.SentAt);

            var options = new CreateIndexOptions
            {
                ExpireAfter = config.SentUpdatesExpiration ?? TimeSpan.FromDays(1)
            };
            var indexModel = new CreateIndexModel<SentUpdate>(keys, options);

            _collection.Indexes.CreateOne(indexModel);
        }
       
        public Task<bool> ExistsAsync(string url)
        {
            return _collection
                .AsQueryable()
                .AnyAsync(sentUpdate => sentUpdate.Url == url);
        }

        public async Task AddAsync(string url)
        {
            var sentUpdate = new SentUpdate
            {
                SentAt = DateTime.Now,
                Url = url
            };
            
            await _collection.InsertOneAsync(sentUpdate);
        }

        public async Task RemoveAsync(string url)
        {
            await _collection.DeleteOneAsync(
                new FilterDefinitionBuilder<SentUpdate>()
                    .Eq(s => s.Url, url));
        }
    }
}