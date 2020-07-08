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
        private readonly ClusterClient _cluster;

        public IObservable<Result<Message<TKey, TValue>>> Messages { get; }

        public Consumer(
            ConsumerConfig config,
            ILoggerFactory loggerFactory)
        {
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

        private static Configuration GetClusterConfig(ConsumerConfig config)
        {
            var serializationConfig = new SerializationConfig();
            
            serializationConfig.SetDefaultDeserializers(
                new KafkaJsonDeserializer<TKey>(), 
                new KafkaJsonDeserializer<TValue>());
            
            return new Configuration
            {
                Seeds = config.BrokersServers,
                SerializationConfig = serializationConfig
            };
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