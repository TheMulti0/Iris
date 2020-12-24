using System;
using System.Reactive.Subjects;
using Common;

namespace UpdatesScraper.Tests
{
    internal class MockUpdatesProducer : IUpdatesProducer
    {
        private readonly Subject<Update> _updates = new();
        public IObservable<Update> Updates => _updates;
        
        public void SendUpdate(Update update)
        {
            _updates.OnNext(update);
        }
    }
}