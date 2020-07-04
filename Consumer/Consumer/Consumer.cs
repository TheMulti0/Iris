using System;
using System.Reactive.Linq;
using System.Text.Json;
using Kafka.Public;
using Kafka.Public.Loggers;

namespace Consumer
{
    public class Consumer<TKey, TValue> : IDisposable
    {
        private readonly ClusterClient _cluster;
        private readonly JsonSerializerOptions _options;

        public IObservable<Result<Message<TKey, TValue>>> Messages { get; }

        public Consumer(
            ConsumerConfig config)
        {
            _options = CreateJsonSerializerOptions();

            _cluster = new ClusterClient(
                GetClusterConfig(config),
                new ConsoleLogger());

            Subscribe(config);

            // TODO Use the built in automatic deserialization instead of manual 
            Messages = _cluster.Messages.Select(ToMessageResult);
        }

        public void Dispose() => _cluster?.Dispose();

        private static JsonSerializerOptions CreateJsonSerializerOptions() => new JsonSerializerOptions
        {
            Converters =
            {
                new DateTimeConverter()
            }
        };

        private static Configuration GetClusterConfig(ConsumerConfig config) => new Configuration
        {
            Seeds = config.BrokersServers
        };

        private void Subscribe(ConsumerConfig config) => _cluster.Subscribe(
            config.GroupId,
            config.Topics,
            new ConsumerGroupConfiguration());

        private Result<Message<TKey, TValue>> ToMessageResult(RawKafkaRecord record)
        {
            try
            {
                return SuccessMessageResult(record);
            }
            catch (Exception e)
            {
                return Result<Message<TKey, TValue>>.Failure(e.Message);
            }
        }

        private Result<Message<TKey, TValue>> SuccessMessageResult(RawKafkaRecord record)
        {
            var message = new Message<TKey, TValue>
            {
                Key = default,
                Value = default,
                Timestamp = record.Timestamp,
                Topic = record.Topic
            };
            if (record.Key != null)
            {
                message.Key = Deserialize<TKey>(record.Key);
            }
            if (record.Value != null)
            {
                message.Value = Deserialize<TValue>(record.Value);
            }

            return Result<Message<TKey, TValue>>.Success(message);
        }

        private T Deserialize<T>(object bytes)
            => JsonSerializer.Deserialize<T>(bytes as byte[], _options);
    }
}