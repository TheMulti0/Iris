using MongoDB.Driver;
using MongoDbGenericRepository;

namespace UpdatesProducer
{
    public class ApplicationDbContext
    {
        public IMongoCollection<UserLatestUpdateTime> UserLatestUpdateTimes { get; }
        public IMongoCollection<SentUpdate> SentUpdates { get; }

        public ApplicationDbContext(IMongoDbContext context)
        {
            UserLatestUpdateTimes = context.GetCollection<UserLatestUpdateTime>();
            SentUpdates = context.GetCollection<SentUpdate>();
        }
    }
}