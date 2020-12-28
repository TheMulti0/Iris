using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

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
        
        public Task<SavedUser> GetAsync(User user)
        {
            (string userId, Platform platform) = user;
            
            return _collection
                .Find(savedUser => savedUser.User.UserId == userId && savedUser.User.Platform == platform)
                .FirstOrDefaultAsync();
        }

        public async Task AddOrUpdateAsync(User user, UserChatInfo chat)
        {
            var savedUser = new SavedUser
            {
                User = user,
                Chats = new List<UserChatInfo> { chat }
            };
            
            SavedUser existing = await GetAsync(user);
            
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

        public async Task RemoveAsync(User user, string chatId)
        {
            var fullUser = await GetAsync(user);
            var chat = fullUser.Chats.First(info => info.ChatId == chatId);

            SavedUser existing = await GetAsync(user);

            if (existing.Chats.Contains(chat) && existing.Chats.Count == 1)
            {
                bool removeSuccess;
                do
                {
                    DeleteResult result = await _collection.DeleteOneAsync(
                        savedUser => savedUser.Version == existing.Version && savedUser.User == user);
                    
                    removeSuccess = result.IsAcknowledged && result.DeletedCount > 0;
                }
                while (!removeSuccess);
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

        private async Task<bool> Update(SavedUser newUser, SavedUser existing)
        {
            IEnumerable<UserChatInfo> combinedChats = existing.Chats
                .Where(info => !newUser.Chats.Contains(info))
                .Concat(newUser.Chats);
            
            UpdateDefinition<SavedUser> update = Builders<SavedUser>.Update
                .Set(u => u.Version, existing.Version + 1)
                .Set(u => u.Chats, combinedChats);

            UpdateResult result = await _collection
                .UpdateOneAsync(
                    savedUser => savedUser.Version == existing.Version && savedUser.User == newUser.User,
                    update);

            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}