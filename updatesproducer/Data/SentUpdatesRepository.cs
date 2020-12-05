using System;
using System.Threading.Tasks;
using Common;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace UpdatesProducer
{
    public class SentUpdatesRepository : ISentUpdatesRepository
    {
        private readonly IMongoCollection<SentUpdate> _collection;

        public SentUpdatesRepository(ApplicationDbContext context)
        {
            _collection = context.SentUpdates;

            var keys = Builders<SentUpdate>.IndexKeys
                .Ascending(update => update.SentAt);
            
            var options = new CreateIndexOptions
            {
                ExpireAfter = TimeSpan.FromDays(1)
            };
            var indexModel = new CreateIndexModel<SentUpdate>(keys, options);
            
            _collection.Indexes
                .CreateOne(indexModel);
        }
        
        public Task<SentUpdate> GetAsync(string url)
        {
            return _collection
                .AsQueryable()
                .FirstOrDefaultAsync(sentUpdate => sentUpdate.Url == url);
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