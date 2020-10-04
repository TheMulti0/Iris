using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using UpdatesConsumer;

namespace DataLayer
{
    public class MongoUpdatesRepository : IUpdatesRepository
    {
        private readonly IMongoCollection<Update> _updates;

        public MongoUpdatesRepository(MongoSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);

            _updates = database.GetCollection<Update>(settings.CollectionName);
        }

        public async Task<IEnumerable<Update>> GetAsync()
        {
            IAsyncCursor<Update> cursor = await _updates.FindAsync(_ => true);
            
            return await cursor.ToListAsync();
        }

        public Task AddAsync(Update update) 
            => _updates.InsertOneAsync(update);

        public Task RemoveAsync(Update update) 
            => _updates.DeleteOneAsync(i => i.Id == update.Id);
    }
}