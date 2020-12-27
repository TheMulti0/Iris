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

        public async Task<ChatId> GetAsync(User user)
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

            var newEntity = await _collection
                .FindOneAndReplaceAsync(
                    c => c.User == user,
                    connection);

            if (newEntity == null)
            {
                await _collection.InsertOneAsync(connection);
            }
        }
    }
}