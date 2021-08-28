using System.Linq;
using System.Threading.Tasks;
using Common;
using MongoDB.Bson;

namespace SubscriptionsDb
{
    public interface IChatSubscriptionsRepository
    {
        Task<bool> ExistsAsync(string userId, string platform);
        
        IQueryable<SubscriptionEntity> Get();
        
        Task<SubscriptionEntity> GetAsync(ObjectId id);
        
        Task<SubscriptionEntity> GetAsync(string userId, string platform);
        
        Task AddOrUpdateAsync(string userId, string platform, UserChatSubscription chat);

        Task RemoveAsync(string userId, string platform, long chatId);
    }
}