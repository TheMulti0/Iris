using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;

namespace UpdatesProducer.Tests
{
    internal class MockUpdatesProvider : IUpdatesProvider
    {
        public Task<IEnumerable<Update>> GetUpdatesAsync(string userId)
        {
            IEnumerable<Update> updates = new Update[]
            {
                new()
                {
                    Content = $"mock update by {userId}",
                    AuthorId = userId,
                    CreationDate = DateTime.Now
                }
            };
            
            return Task.FromResult(updates);
        }
    }
}