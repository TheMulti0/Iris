using System;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using Kafka.Public;
using Microsoft.Extensions.Logging;

namespace Extensions
{
    public class Consumer<TKey, TValue> : IDisposable
    {
        private readonly IClusterClient _cluster;

        public IObservable<Result<Message<TKey, TValue>>> Messages { get; }

        public Consumer(
            ConsumerConfig config,
            ILoggerFactory loggerFactory)
        {
            _cluster = ClusterClientFactory.CreateClusterClient(
                config,
                CreateSerializationConfig(config),
                loggerFactory);

            Subscribe(config);

            Messages = _cluster.Messages.Select(ToMessageResult);
        }

        public void Dispose() => _cluster?.Dispose();

        private static SerializationConfig CreateSerializationConfig(BaseKafkaConfig config)
        {
            var serializationConfig = new SerializationConfig();

            serializationConfig.SetDefaultDeserializers(
                KafkaDeserializerFactory.CreateDeserializer<TKey>(config.KeySerializationType),
                KafkaDeserializerFactory.CreateDeserializer<TValue>(config.ValueSerializationType));
            
            return serializationConfig;
        }

        private void Subscribe(ConsumerConfig config)
        {
            _cluster.Subscribe(
                config.GroupId,
                config.Topics,
                new ConsumerGroupConfiguration());
        }

        private static Result<Message<TKey, TValue>> ToMessageResult(RawKafkaRecord record)
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

        private static Result<Message<TKey, TValue>> SuccessMessageResult(RawKafkaRecord record)
        {
            object key = record.Key;
            object value = record.Value;
           
            var message = new Message<TKey, TValue>
            {
                Key = Optional<TKey>.CreateIfInstanceOf(key),
                Value = Optional<TValue>.CreateIfInstanceOf(value),
                Timestamp = record.Timestamp,
                Topic = record.Topic
            };
            
            return Result<Message<TKey, TValue>>.Success(message);
        }
    }
}