using MongoDB.Driver;
using MongoDbGenericRepository;

namespace TelegramReceiver
{
    internal class MongoApplicationDbContext
    {
        public IMongoCollection<Connection> Connection { get; }

        public MongoApplicationDbContext(IMongoDbContext context)
        {
            Connection = context.GetCollection<Connection>();
        }
    }
}