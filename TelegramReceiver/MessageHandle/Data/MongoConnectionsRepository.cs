using System.Threading.Tasks;
using Common;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Telegram.Bot.Types;
using User = Telegram.Bot.Types.User;

namespace TelegramReceiver.Data
{
    internal class MongoConnectionsRepository : IConnectionsRepository
    {
        private readonly IMongoCollection<Connection> _collection;

        public MongoConnectionsRepository(
            MongoApplicationDbContext context)
        {
            _collection = context.Connection;
        }

        public Task<Connection> GetAsync(User user)
        {
            return _collection.AsQueryable()
                .FirstOrDefaultAsync(c => c.User.Id == user.Id);
        }

        public async Task AddOrUpdateAsync(User user, ChatId chatId, Language language)
        {
            var connection = new Connection
            {
                User = user,
                Chat = chatId
            };

            var existingConnection = await GetAsync(user);
            string existingChatId = existingConnection?.Chat;
            
            if (existingChatId == null)
            {
                await _collection.InsertOneAsync(connection);
                return;
            }

            UpdateDefinition<Connection> update = Builders<Connection>.Update
                .Set(c => c.Chat, (string) chatId)
                .Set(c => c.Language, language);
            
            await _collection.UpdateOneAsync(
                c => c.User == user,
                update);
        }
    }
}