using System.Threading.Tasks;
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
        }
        
        public Task<SentUpdate> GetAsync(string url)
        {
            return _collection
                .AsQueryable()
                .FirstOrDefaultAsync(sentUpdate => sentUpdate.Url == url);
        }

        public async Task SetAsync(string url)
        {
            var sentUpdate = new SentUpdate
            {
                Url = url
            };
            
            var newEntity = await _collection
                .FindOneAndReplaceAsync(
                    u => u.Url == url,
                    sentUpdate);

            if (newEntity == null)
            {
                await _collection.InsertOneAsync(sentUpdate);
            }
        }
    }
}