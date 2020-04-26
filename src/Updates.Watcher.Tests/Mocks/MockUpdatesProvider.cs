using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Iris.Api;

namespace Updates.Watcher.Tests
{
    internal class MockUpdatesProvider : IUpdatesProvider
    {
        public Task<IEnumerable<Update>> GetUpdates(string userName)
        {
            IEnumerable<Update> updates = new[]
            {
                new Update(
                    0,
                    new User(
                        "0",
                        userName,
                        "",
                        ""),
                    "",
                    "",
                    DateTime.Now,
                    "",
                    new List<Media>())
            };
            
            return Task.FromResult(updates);
        }
    }
}