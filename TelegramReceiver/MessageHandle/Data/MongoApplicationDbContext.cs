using MongoDB.Driver;
using MongoDbGenericRepository;

namespace TelegramReceiver.Data
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