using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;

namespace UpdatesScraper.Tests
{
    internal class MockUpdatesProvider : IUpdatesProvider
    {
        public Task<IEnumerable<Update>> GetUpdatesAsync(User user)
        {
            IEnumerable<Update> updates = new Update[]
            {
                new()
                {
                    Content = $"mock update by {user}",
                    Author = user,
                    CreationDate = DateTime.Now,
                    Url = "mockurl.com"
                }
            };
            
            return Task.FromResult(updates);
        }
    }
}