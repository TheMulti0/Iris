using System;
using System.IO;
using System.Reactive.Subjects;
using System.Text.Json;
using Extensions;

namespace TelegramConsumer.Tests
{
    internal class FileConfigsProvider : IConfigsProvider
    {
        private readonly Subject<Result<TelegramConfig>> _configs = new Subject<Result<TelegramConfig>>();
        public IObservable<Result<TelegramConfig>> Configs => _configs;
        
        public void InitializeSubscriptions()
        {
            var config = JsonSerializer.Deserialize<TelegramConfig>(
                File.ReadAllText("../../../telegramconfig.json"));
            
            _configs.OnNext(Result<TelegramConfig>.Success(config));
        }
    }
}