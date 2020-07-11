using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;
using Extensions;
using Microsoft.Extensions.Logging;

namespace TelegramConsumer
{
    public class ConfigsProvider
    {
        private readonly Consumer<string, string> _consumer;
        private readonly TelegramConfig _defaultConfig;
        private readonly ILogger<ConfigsProvider> _logger;
        
        private readonly Subject<Result<TelegramConfig>> _configs;
        public IObservable<Result<TelegramConfig>> Configs => _configs;

        public ConfigsProvider(
            Consumer<string, string> consumer,
            TelegramConfig defaultConfig,
            ILogger<ConfigsProvider> logger)
        {
            _consumer = consumer;
            _defaultConfig = defaultConfig;
            _logger = logger;
            _configs = new Subject<Result<TelegramConfig>>();
        }

        public void InitializeSubscriptions()
        {
            if (_defaultConfig != null)
            {
                _logger.LogInformation("Found default Telegram config, sending..");
                _configs.OnNext(Result<TelegramConfig>.Success(_defaultConfig));
            }
            
            _consumer.Messages
                .Where(ConfigBelongsToTelegram)
                .Select(DeserializeConfig)
                .Subscribe(_configs.OnNext);
        }
        
        private static bool ConfigBelongsToTelegram(Result<Message<string, string>> result)
        {
            return result.Value?.Key.ValueEqualsTo("Telegram") ?? false;
        }

        private static Result<TelegramConfig> DeserializeConfig(Result<Message<string, string>> result)
        {
            return result.Map(
                message => JsonSerializer.Deserialize<TelegramConfig>(message.Value.Value));
        }
    }
}