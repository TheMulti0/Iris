using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Updates.Api;

namespace Updates.Watcher.Tests
{
    internal class MockUpdatesProvider : IUpdatesProvider
    {
        public Task<IEnumerable<IUpdate>> GetUpdates(long authorId)
        {
            IEnumerable<IUpdate> updates = new[]
            {
                new MockUpdate(
                    0,
                    "",
                    new MockUser(
                        authorId,
                        "",
                        "",
                        ""),
                    DateTime.Now,
                    "")
            };
            
            return Task.FromResult(updates);
        }
    }
}