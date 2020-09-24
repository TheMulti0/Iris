using System;
using Extensions;

namespace TelegramBot
{
    public interface IConfigProvider
    {
        public IObservable<Result<TelegramConfig>> Configs { get; }
    }
}