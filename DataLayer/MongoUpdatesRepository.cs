using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using UpdatesConsumer;

namespace DataLayer
{
    public class MongoUpdatesRepository : IUpdatesRepository
    {
        private readonly IMongoCollection<Update> _updates;

        public MongoUpdatesRepository(string connectionString, string databaseName, string collection)
        {
            var client = new MongoClient(connectionString);
            IMongoDatabase database = client.GetDatabase(databaseName);

            _updates = database.GetCollection<Update>(collection);
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