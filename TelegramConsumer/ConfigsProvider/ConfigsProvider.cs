using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;
using Extensions;
using Kafka.Public;
using Microsoft.Extensions.Logging;

namespace TelegramConsumer
{
    public class ConfigsProvider
    {
        private readonly IKafkaConsumer<string, string> _consumer;
        private readonly TelegramConfig _defaultConfig;
        private readonly ILogger<ConfigsProvider> _logger;
        
        private readonly Subject<Result<TelegramConfig>> _configs;
        public IObservable<Result<TelegramConfig>> Configs => _configs;

        public ConfigsProvider(
            IKafkaConsumer<string, string> consumer,
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
        
        private static bool ConfigBelongsToTelegram(KafkaRecord<string, string> record)
        {
            return record.Key == "Telegram";
        }

        private static Result<TelegramConfig> DeserializeConfig(KafkaRecord<string, string> record)
        {
            try
            {
                return Result<TelegramConfig>.Success(
                    JsonSerializer.Deserialize<TelegramConfig>(record.Value));
            }
            catch (Exception e)
            {
                return Result<TelegramConfig>.Failure(e);
            }
        }
    }
}