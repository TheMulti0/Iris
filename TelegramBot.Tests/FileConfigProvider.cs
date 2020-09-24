using System;
using System.IO;
using System.Reactive.Subjects;
using System.Text.Json;
using Extensions;

namespace TelegramBot.Tests
{
    internal class FileConfigProvider : IConfigProvider
    {
        private readonly BehaviorSubject<Result<TelegramConfig>> _configs;
        public IObservable<Result<TelegramConfig>> Configs => _configs;

        public FileConfigProvider()
        {
            var config = JsonSerializer.Deserialize<TelegramConfig>(
                File.ReadAllText("../../../telegramconfig.json"));
            _configs = new BehaviorSubject<Result<TelegramConfig>>(Result<TelegramConfig>.Success(config));
        }
    }
}