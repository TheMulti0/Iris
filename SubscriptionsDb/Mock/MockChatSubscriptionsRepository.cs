using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using MongoDB.Bson;

namespace SubscriptionsDb
{
    public class MockChatSubscriptionsRepository : IChatSubscriptionsRepository
    {
        public Task<bool> ExistsAsync(User user) => Task.FromResult(false);

        public IQueryable<SubscriptionEntity> Get()
        {
            return new EnumerableQuery<SubscriptionEntity>(
                new SubscriptionEntity[]
                {
                    new()
                    {
                        User = new User("user", Platform.Facebook),
                        Chats = new List<UserChatSubscription>()
                    }
                });
        }

        public Task<SubscriptionEntity> GetAsync(ObjectId id)
        {
            return Task.FromResult(
                new SubscriptionEntity
                {
                    Chats = new List<UserChatSubscription>()
                });
        }

        public Task<SubscriptionEntity> GetAsync(User user)
        {
            return Task.FromResult(
                new SubscriptionEntity
                {
                    User = user,
                    Chats = new List<UserChatSubscription>()
                });
        }

        public Task AddOrUpdateAsync(User user, UserChatSubscription chat)
        {
            return Task.CompletedTask;
        }

        public Task RemoveAsync(User user, long chatId)
        {
            return Task.CompletedTask;
        }
    }
}