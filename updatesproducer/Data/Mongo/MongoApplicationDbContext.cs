using MongoDB.Driver;
using MongoDbGenericRepository;

namespace UpdatesProducer
{
    public class MongoApplicationDbContext
    {
        public IMongoCollection<UserLatestUpdateTime> UserLatestUpdateTimes { get; }
        public IMongoCollection<SentUpdate> SentUpdates { get; }

        public MongoApplicationDbContext(IMongoDbContext context)
        {
            UserLatestUpdateTimes = context.GetCollection<UserLatestUpdateTime>();
            SentUpdates = context.GetCollection<SentUpdate>();
        }
    }
}