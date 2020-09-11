using System;
using System.Reactive.Subjects;
using Extensions;

namespace TelegramConsumer.Tests
{
    internal class MockConfigProvider : IConfigProvider
    {
        private static readonly TelegramConfig _telegramConfig = new TelegramConfig
        {
            AccessToken = "",
            Users = new []
            {
                new User
                {
                    UserName = "mock-user",
                    DisplayName = "Mock User",
                    ChatIds = new long[] { 0 }
                }
            }
        };
        
        private readonly BehaviorSubject<Result<TelegramConfig>> _configs = new BehaviorSubject<Result<TelegramConfig>>(Result<TelegramConfig>.Success(_telegramConfig));

        public IObservable<Result<TelegramConfig>> Configs => _configs;
    }
}