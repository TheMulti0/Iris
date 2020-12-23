using System;

namespace IrisPoc
{
    internal interface IMessagesProducer
    {
        IObservable<Message> Messages { get; }
    }
}