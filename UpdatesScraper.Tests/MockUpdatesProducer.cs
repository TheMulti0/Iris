using System;
using System.Reactive.Subjects;
using Common;
using Extensions;

namespace UpdatesScraper.Tests
{
    internal class MockUpdatesProducer : IProducer<Update>
    {
        private readonly Subject<Update> _updates = new();
        public IObservable<Update> Updates => _updates;
        
        public void Send(Update update)
        {
            _updates.OnNext(update);
        }
    }
}