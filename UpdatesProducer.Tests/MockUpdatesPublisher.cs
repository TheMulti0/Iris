using System;
using System.Reactive.Subjects;
using Common;

namespace UpdatesProducer.Tests
{
    internal class MockUpdatesPublisher : IUpdatesPublisher
    {
        private readonly Subject<Update> _updates = new();
        public IObservable<Update> Updates => _updates;
        
        public void Send(Update update)
        {
            _updates.OnNext(update);
        }
    }
}