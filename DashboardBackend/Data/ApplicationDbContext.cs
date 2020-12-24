using MongoDB.Driver;
using MongoDbGenericRepository;

namespace DashboardBackend.Data
{
    public class ApplicationDbContext
    {
        public IMongoCollection<UpdateEntity> Updates { get; }

        public ApplicationDbContext(IMongoDbContext context)
        {
            Updates = context.GetCollection<UpdateEntity>();
        }
    }
}