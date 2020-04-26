using System;
using System.Reactive.Subjects;

namespace Iris.Watcher.Tests
{
    internal class MockUpdatesValidator : IUpdatesValidator
    {
        public IObservable<(long updateId, long chatId)> SentUpdates { get; } = new Subject<(long updateId, long chatId)>();
        
        public bool WasUpdateSent(long updateId, long chatId) => false;

        public void UpdateSent(long updateId, long chatId) { }
    }
}