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
                    UserNames = new [] {"mock-user"},
                    DisplayName = "Mock User",
                    ChatIds = new[] { 0.ToString() }
                }
            }
        };
        
        private readonly BehaviorSubject<Result<TelegramConfig>> _configs = new BehaviorSubject<Result<TelegramConfig>>(Result<TelegramConfig>.Success(_telegramConfig));

        public IObservable<Result<TelegramConfig>> Configs => _configs;
    }
}