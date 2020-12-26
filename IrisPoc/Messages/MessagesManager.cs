using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using Common;

namespace IrisPoc
{
    internal class MessagesManager : IUpdatesConsumer, IMessagesProducer
    {
        private readonly Subject<Message> _messages = new();
        public IObservable<Message> Messages => _messages;

        private readonly IDataLayer _dataLayer;

        public MessagesManager(IDataLayer dataLayer)
        {
            _dataLayer = dataLayer;
        }

        public void NewUpdate(Update update)
        {
            List<string> chatIds = _dataLayer.Get()
                .FirstOrDefault(pair => pair.Key.User.UserId == update.Author.UserId)
                .Value;
            
            _messages.OnNext(new Message(update, chatIds));
        }
    }
}