using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDbGenericRepository;

namespace SubscriptionsDb
{
    public class MongoChatSubscriptionsRepository : IChatSubscriptionsRepository
    {
        private readonly IMongoCollection<SubscriptionEntity> _collection;

        public MongoChatSubscriptionsRepository(IMongoDbContext context)
        {
            _collection = context.GetCollection<SubscriptionEntity>();
        }

        public Task<bool> ExistsAsync(string userId, string platform)
        {
            return _collection
                .AsQueryable()
                .Where(subscription => subscription.UserId == userId && subscription.Platform == platform)
                .AnyAsync();
        }

        public IQueryable<SubscriptionEntity> Get()
        {
            return _collection
                .AsQueryable();
        }

        public Task<SubscriptionEntity> GetAsync(ObjectId id)
        {
            return _collection
                .AsQueryable()
                .Where(subscription => subscription.Id == id)
                .FirstOrDefaultAsync();
        }

        public Task<SubscriptionEntity> GetAsync(string userId, string platform)
        {
            return _collection
                .AsQueryable()
                .Where(subscription => subscription.UserId == userId && subscription.Platform == platform)
                .FirstOrDefaultAsync();
        }

        public async Task AddOrUpdateAsync(string userId, string platform, UserChatSubscription chat)
        {
            SubscriptionEntity existing = await GetAsync(userId, platform);

            List<UserChatSubscription> userChatSubscriptions = GetSubscriptions(existing, chat);

            var subscription = new SubscriptionEntity
            {
                UserId = userId,
                Platform = platform,
                Chats = userChatSubscriptions
            };
            
            if (existing == null)
            {
                await _collection.InsertOneAsync(subscription);
                return;
            }

            bool updateSuccess;
            do
            {
                updateSuccess = await Update(
                    subscription,
                    await GetAsync(userId, platform));
            }
            while (!updateSuccess);
        }

        private static List<UserChatSubscription> GetSubscriptions(
            SubscriptionEntity existing,
            UserChatSubscription chat)
        {
            var thisChat = new List<UserChatSubscription> { chat };

            if (existing == null)
            {
                return thisChat;
            }

            UserChatSubscription existingChat = existing.Chats.FirstOrDefault(c => c.ChatInfo.Id == chat.ChatInfo.Id);
            
            if (existingChat == null)
            {
                return existing.Chats.Concat(thisChat).ToList();
            }
            
            existing.Chats[existing.Chats.IndexOf(existingChat)] = chat;

            return existing.Chats;
        }

        public async Task RemoveAsync(string userId, string platform, long chatId)
        {
            SubscriptionEntity existing = await GetAsync(userId, platform);
            if (existing == null)
            {
                return;
            }
            
            UserChatSubscription chat = existing.Chats.First(info => info.ChatInfo.Id == chatId);

            if (existing.Chats.Contains(chat) && existing.Chats.Count == 1)
            {
                await Remove(userId, platform, existing);

                return;
            }

            existing.Chats.Remove(chat);

            bool updateSuccess;
            do
            {
                updateSuccess = await Update(
                    existing,
                    await GetAsync(userId, platform));
            }
            while (!updateSuccess);
        }

        private async Task Remove(string userId, string platform, SubscriptionEntity existing)
        {
            bool removeSuccess;
            do
            {
                existing = await GetAsync(userId, platform);

                DeleteResult result = await _collection.DeleteOneAsync(
                    s => s.Version == existing.Version && s.UserId == userId && s.Platform == platform);

                removeSuccess = result.IsAcknowledged && result.DeletedCount > 0;
            }
            while (!removeSuccess);
        }

        private async Task<bool> Update(
            SubscriptionEntity newUser,
            SubscriptionEntity existing)
        {
            int version = GetVersion(existing);

            UpdateDefinition<SubscriptionEntity> update = Builders<SubscriptionEntity>.Update
                .Set(u => u.Version, version + 1)
                .Set(u => u.Chats, newUser.Chats);

            UpdateResult result = await _collection
                .UpdateOneAsync(
                    subscription => subscription.Version == version && subscription.UserId == newUser.UserId && subscription.Platform == newUser.Platform,
                    update);

            if (result.IsAcknowledged)
            {
                return result.MatchedCount > 0;
            }
            
            return false;
        }

        private static int GetVersion(SubscriptionEntity existing)
        {
            if (existing == null)
            {
                return 0;
            }
            
            return existing.Version;
        }
    }
}