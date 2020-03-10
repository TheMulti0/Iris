using System;
using ProducerApi;

namespace ConsumerTelegramBot
{
    internal interface IUsersWatcher
    {
        IObservable<IUpdate> Updates { get; }
    }
}