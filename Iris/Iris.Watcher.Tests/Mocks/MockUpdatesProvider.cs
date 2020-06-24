using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Iris.Api;

namespace Iris.Watcher.Tests
{
    internal class MockUpdatesProvider : IUpdatesProvider
    {
        public Task<IEnumerable<Update>> GetUpdates(User user)
        {
            IEnumerable<Update> updates = new[]
            {
                new Update(
                    0,
                    user,
                    "",
                    DateTime.Now,
                    "",
                    new List<Media>())
            };
            
            return Task.FromResult(updates);
        }
    }
}