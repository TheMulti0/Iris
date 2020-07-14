using System;
using Extensions;

namespace TelegramConsumer
{
    public interface IConfigsProvider
    {
        public IObservable<Result<TelegramConfig>> Configs { get; }

        void InitializeSubscriptions();
    }
}