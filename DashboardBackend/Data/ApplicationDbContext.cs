using MongoDB.Driver;
using MongoDbGenericRepository;
using UpdatesConsumer;

namespace DashboardBackend.Data
{
    public class ApplicationDbContext
    {
        public IMongoCollection<Update> Updates { get; }

        public ApplicationDbContext(IMongoDbContext context)
        {
            Updates = context.GetCollection<Update>();
        }
    }
}