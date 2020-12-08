using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace UpdatesProducer
{
    public class MongoSentUpdatesRepository : ISentUpdatesRepository
    {
        private readonly IMongoCollection<SentUpdate> _collection;

        public MongoSentUpdatesRepository(
            MongoApplicationDbContext context,
            MongoDbConfig config)
        {
            _collection = context.SentUpdates;

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
    }
}