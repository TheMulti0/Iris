using System.Linq;
using System.Threading.Tasks;
using Common;
using MongoDB.Bson;

namespace SubscriptionsDb
{
    public interface IChatSubscriptionsRepository
    {
        Task<bool> ExistsAsync(User user);
        
        IQueryable<SubscriptionEntity> Get();
        
        Task<SubscriptionEntity> GetAsync(ObjectId id);
        
        Task<SubscriptionEntity> GetAsync(User user);
        
        Task AddOrUpdateAsync(User user, UserChatSubscription chat);

        Task RemoveAsync(User user, long chatId);
    }
}