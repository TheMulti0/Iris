using System;
using System.Reactive.Subjects;
using Extensions;

namespace TelegramConsumer.Tests
{
    internal class MockConfigProvider : IConfigProvider
    {
        private readonly Subject<Result<TelegramConfig>> _configs = new Subject<Result<TelegramConfig>>();
        public IObservable<Result<TelegramConfig>> Configs => _configs;
        
        public void InitializeSubscriptions()
        {
            var telegramConfig = new TelegramConfig
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
            
            _configs.OnNext(Result<TelegramConfig>.Success(telegramConfig));
        }
    }
}