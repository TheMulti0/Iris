using MongoDB.Driver;
using MongoDbGenericRepository;

namespace SubscriptionsDataLayer
{
    public class MongoApplicationDbContext
    {
        public IMongoCollection<SubscriptionEntity> Subscriptions { get; }
        
        public MongoApplicationDbContext(IMongoDbContext context)
        {
            Subscriptions = context.GetCollection<SubscriptionEntity>();
        }
    }
}