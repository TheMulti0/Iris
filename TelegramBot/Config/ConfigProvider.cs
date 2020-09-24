using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;
using Extensions;
using Kafka.Public;
using Microsoft.Extensions.Logging;

namespace TelegramBot
{
    internal class ConfigProvider : IConfigProvider
    {
        private readonly BehaviorSubject<Result<TelegramConfig>> _configs;
        public IObservable<Result<TelegramConfig>> Configs => _configs;

        public ConfigProvider(
            IKafkaConsumer<string, string> consumer,
            TelegramConfig defaultConfig)
        {
            _configs = new BehaviorSubject<Result<TelegramConfig>>(Result<TelegramConfig>.Success(defaultConfig));
            
            consumer.Messages
                .Where(ConfigBelongsToTelegram)
                .Select(DeserializeConfig)
                .Subscribe(_configs.OnNext);
        }
        
        private static bool ConfigBelongsToTelegram(KafkaRecord<string, string> record) 
            => record.Key == "Telegram";

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