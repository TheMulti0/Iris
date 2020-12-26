using MongoDB.Driver;
using MongoDbGenericRepository;

namespace UserDataLayer
{
    public class MongoApplicationDbContext
    {
        public IMongoCollection<SavedUser> SavedUsers { get; }
        
        public MongoApplicationDbContext(IMongoDbContext context)
        {
            SavedUsers = context.GetCollection<SavedUser>();
        }
    }
}