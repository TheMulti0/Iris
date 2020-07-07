using System;
using System.Reactive.Linq;
using System.Text.Json;
using Kafka.Public;
using Microsoft.Extensions.Logging;

namespace Extensions
{
    public class Consumer<TKey, TValue> : IDisposable
    {
        private readonly ClusterClient _cluster;
        private readonly JsonSerializerOptions _options;

        public IObservable<Result<Message<TKey, TValue>>> Messages { get; }

        public Consumer(
            ConsumerConfig config,
            ILoggerFactory loggerFactory)
        {
            _options = CreateJsonSerializerOptions();

            var logger = new KafkaSharpMicrosoftLogger(
                loggerFactory.CreateLogger("kafka-sharp"));
            
            _cluster = new ClusterClient(
                GetClusterConfig(config),
                logger);

            Subscribe(config);

            // TODO Use the built in automatic deserialization instead of manual 
            Messages = _cluster.Messages.Select(ToMessageResult);
        }

        public void Dispose() => _cluster?.Dispose();

        private static JsonSerializerOptions CreateJsonSerializerOptions()
        {
            return new JsonSerializerOptions
            {
                Converters =
                {
                    new DateTimeConverter()
                }
            };
        }

        private static Configuration GetClusterConfig(ConsumerConfig config)
        {
            return new Configuration
            {
                Seeds = config.BrokersServers
            };
        }

        private void Subscribe(ConsumerConfig config)
        {
            _cluster.Subscribe(
                config.GroupId,
                config.Topics,
                new ConsumerGroupConfiguration());
        }

        private Result<Message<TKey, TValue>> ToMessageResult(RawKafkaRecord record)
        {
            try
            {
                return SuccessMessageResult(record);
            }
            catch (Exception e)
            {
                return Result<Message<TKey, TValue>>.Failure(e);
            }
        }

        private Result<Message<TKey, TValue>> SuccessMessageResult(RawKafkaRecord record)
        {
            var key = record.Key;
            var value = record.Value;
           
            var message = new Message<TKey, TValue>
            {
                Key = Deserialize<TKey>(key),
                Value = Deserialize<TValue>(value),
                Timestamp = record.Timestamp,
                Topic = record.Topic
            };
            
            return Result<Message<TKey, TValue>>.Success(message);
        }

        private Optional<T> Deserialize<T>(object bytes)
        {
            return bytes == null
                ? Optional<T>.Empty()
                : Optional<T>.WithValue(
                    JsonSerializer.Deserialize<T>(bytes as byte[], _options));
        }
    }
}