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

        public Task<bool> ExistsAsync(User user)
        {
            (string userId, Platform platform) = user;
            
            return _collection
                .AsQueryable()
                .Where(subscription => subscription.User.UserId == userId && subscription.User.Platform == platform)
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

        public Task<SubscriptionEntity> GetAsync(User user)
        {
            (string userId, Platform platform) = user;
            
            return _collection
                .AsQueryable()
                .Where(subscription => subscription.User.UserId == userId && subscription.User.Platform == platform)
                .FirstOrDefaultAsync();
        }

        public async Task AddOrUpdateAsync(User user, UserChatSubscription chat)
        {
            SubscriptionEntity existing = await GetAsync(user);

            List<UserChatSubscription> userChatSubscriptions = GetSubscriptions(existing, chat);

            var subscription = new SubscriptionEntity
            {
                User = user,
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
                    await GetAsync(user));
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

        public async Task RemoveAsync(User user, long chatId)
        {
            SubscriptionEntity existing = await GetAsync(user);
            if (existing == null)
            {
                return;
            }
            
            UserChatSubscription chat = existing.Chats.First(info => info.ChatInfo.Id == chatId);

            if (existing.Chats.Contains(chat) && existing.Chats.Count == 1)
            {
                await Remove(user, existing);

                return;
            }

            existing.Chats.Remove(chat);

            bool updateSuccess;
            do
            {
                updateSuccess = await Update(
                    existing,
                    await GetAsync(user));
            }
            while (!updateSuccess);
        }

        private async Task Remove(User user, SubscriptionEntity existing)
        {
            bool removeSuccess;
            do
            {
                existing = await GetAsync(user);

                DeleteResult result = await _collection.DeleteOneAsync(
                    s => s.Version == existing.Version && s.User == user);

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
                    subscription => subscription.Version == version && subscription.User == newUser.User,
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