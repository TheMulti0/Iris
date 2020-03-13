using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Updates.Api;

namespace Updates.Watcher.Tests
{
    internal class MockUpdatesProvider : IUpdatesProvider
    {
        public Task<IEnumerable<Update>> GetUpdates(long authorId)
        {
            IEnumerable<Update> updates = new[]
            {
                new Update(
                    0,
                    "",
                    new User(
                        authorId,
                        "",
                        "",
                        ""),
                    DateTime.Now,
                    "",
                    "")
            };
            
            return Task.FromResult(updates);
        }
    }
}