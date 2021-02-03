using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using MongoDB.Bson;
using MongoDB.Driver;

namespace UserDataLayer
{
    public class MongoSavedUsersRepository : ISavedUsersRepository
    {
        private readonly IMongoCollection<SavedUser> _collection;

        public MongoSavedUsersRepository(MongoApplicationDbContext context)
        {
            _collection = context.SavedUsers;
        }

        public IQueryable<SavedUser> GetAll()
        {
            return _collection
                .AsQueryable();
        }

        public Task<SavedUser> GetAsync(ObjectId id)
        {
            return _collection
                .Find(savedUser => savedUser.Id == id)
                .FirstOrDefaultAsync();
        }

        public Task<SavedUser> GetAsync(User user)
        {
            (string userId, Platform platform) = user;
            
            return _collection
                .Find(savedUser => savedUser.User.UserId == userId && savedUser.User.Platform == platform)
                .FirstOrDefaultAsync();
        }

        public async Task AddOrUpdateAsync(User user, UserChatSubscription chat)
        {
            SavedUser existing = await GetAsync(user);

            List<UserChatSubscription> userChatSubscriptions = GetSubscriptions(existing, chat);

            var savedUser = new SavedUser
            {
                User = user,
                Chats = userChatSubscriptions
            };
            
            if (existing == null)
            {
                await _collection.InsertOneAsync(savedUser);
                return;
            }

            bool updateSuccess;
            do
            {
                updateSuccess = await Update(
                    savedUser,
                    await GetAsync(user));
            }
            while (!updateSuccess);
        }

        private static List<UserChatSubscription> GetSubscriptions(
            SavedUser existing,
            UserChatSubscription chat)
        {
            var thisChat = new List<UserChatSubscription> { chat };

            if (existing == null)
            {
                return thisChat;
            }

            if (!existing.Chats.Contains(chat))
            {
                return existing.Chats.Concat(thisChat).ToList();
            }
            
            existing.Chats[existing.Chats.IndexOf(chat)] = chat;

            return existing.Chats;
        }

        public async Task RemoveAsync(User user, string chatId)
        {
            SavedUser existing = await GetAsync(user);
            if (existing == null)
            {
                return;
            }
            
            UserChatSubscription chat = existing.Chats.First(info => info.ChatId == chatId);

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

        private async Task Remove(User user, SavedUser existing)
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

        private async Task<bool> Update(SavedUser newUser, SavedUser existing)
        {
            UpdateDefinition<SavedUser> update = Builders<SavedUser>.Update
                .Set(u => u.Version, existing.Version + 1)
                .Set(u => u.Chats, newUser.Chats);

            UpdateResult result = await _collection
                .UpdateOneAsync(
                    savedUser => savedUser.Version == existing.Version && savedUser.User == newUser.User,
                    update);

            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}