using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Telegram.Bot.Types;

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

        public async Task<string> GetAsync(User user)
        {
            Connection connection = await _collection.AsQueryable()
                .FirstOrDefaultAsync(c => c.User.Id == user.Id);
            
            return connection?.Chat;
        }

        public async Task AddOrUpdateAsync(User user, ChatId chatId)
        {
            var connection = new Connection
            {
                User = user,
                Chat = chatId
            };

            string existingChatId = await GetAsync(user);
            if (existingChatId == chatId)
            {
                return;
            }
            if (existingChatId == null)
            {
                await _collection.InsertOneAsync(connection);
                return;
            }

            UpdateDefinition<Connection> update = Builders<Connection>.Update.Set(c => c.Chat, (string) chatId);
            
            await _collection.UpdateOneAsync(
                c => c.User == user,
                update);
        }
    }
}