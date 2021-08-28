using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using MongoDB.Bson;

namespace SubscriptionsDb
{
    public class MockChatSubscriptionsRepository : IChatSubscriptionsRepository
    {
        public Task<bool> ExistsAsync(string userId, string platform) => Task.FromResult(false);

        public IQueryable<SubscriptionEntity> Get()
        {
            return new EnumerableQuery<SubscriptionEntity>(
                new SubscriptionEntity[]
                {
                    new()
                    {
                        UserId = "user",
                        Platform = "facebook",
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

        public Task<SubscriptionEntity> GetAsync(string userId, string platform)
        {
            return Task.FromResult(
                new SubscriptionEntity
                {
                    UserId = userId,
                    Platform = platform,
                    Chats = new List<UserChatSubscription>()
                });
        }

        public Task AddOrUpdateAsync(string userId, string platform, UserChatSubscription chat)
        {
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string userId, string platform, long chatId)
        {
            return Task.CompletedTask;
        }
    }
}