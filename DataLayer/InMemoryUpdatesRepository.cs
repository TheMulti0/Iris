using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using UpdatesConsumer;

namespace DataLayer
{
    public class InMemoryUpdatesRepository : IUpdatesRepository
    {
        private readonly ConcurrentDictionary<string, Update> _updates = new ConcurrentDictionary<string, Update>();
        
        public async Task<IEnumerable<Update>> GetAsync() => _updates.Values;

        public async Task AddAsync(Update update)
        {
            _updates.TryAdd(ToJson(update), update);
        }

        public async Task RemoveAsync(Update update)
        {
            _updates.TryRemove(ToJson(update), out update);
        }

        private static string ToJson(Update item) 
            => JsonSerializer.Serialize(item);
    }
}