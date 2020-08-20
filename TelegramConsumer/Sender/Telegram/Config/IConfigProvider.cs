using System;
using Extensions;

namespace TelegramConsumer
{
    public interface IConfigProvider
    {
        public IObservable<Result<TelegramConfig>> Configs { get; }
    }
}