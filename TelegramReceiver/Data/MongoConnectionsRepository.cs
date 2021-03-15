using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDbGenericRepository;
using User = Telegram.Bot.Types.User;

namespace TelegramReceiver
{
    internal class MongoConnectionsRepository : IConnectionsRepository
    {
        private readonly IMongoCollection<Connection> _collection;

        public MongoConnectionsRepository(IMongoDbContext context)
        {
            _collection = context.GetCollection<Connection>();
        }

        public Task<Connection> GetAsync(User user)
        {
            return _collection.AsQueryable()
                .FirstOrDefaultAsync(c => c.User.Id == user.Id);
        }

        public async Task AddOrUpdateAsync(User user, IConnectionProperties properties)
        {
            var existingConnection = await GetAsync(user);
            var connection = new Connection(properties)
            {
                User = user
            };
            
            if (existingConnection == null)
            {
                await _collection.InsertOneAsync(connection);
                return;
            }

            UpdateDefinition<Connection> update = Builders<Connection>.Update
                .Set(c => c.Chat, connection.Chat)
                .Set(c => c.Language, connection.Language)
                .Set(c => c.HasAgreedToTos, connection.HasAgreedToTos);
            
            await _collection.UpdateOneAsync(
                c => c.User == user,
                update);
        }
    }
}