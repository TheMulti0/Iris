using System;

namespace ProducerApi
{
    public interface IProducer
    {
        IObservable<IUpdate> Updates { get; }
    }
}
