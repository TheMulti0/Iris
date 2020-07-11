using System;
using Kafka.Public;
using Microsoft.Extensions.Logging;

namespace Extensions
{
    public class Producer<TKey, TValue> : IDisposable
    {
        private readonly IClusterClient _cluster;
        private readonly ILogger<Producer<TKey, TValue>> _logger;

        public Producer(
            BaseKafkaConfig config,
            ILoggerFactory loggerFactory)
        {
            _cluster = ClusterClientFactory.Create(
                config,
                CreateSerializationConfig(config),
                loggerFactory);

            _logger = loggerFactory.CreateLogger<Producer<TKey, TValue>>();
        }

        public void Dispose() => _cluster?.Dispose();

        private static SerializationConfig CreateSerializationConfig(BaseKafkaConfig config)
        {
            var serializationConfig = new SerializationConfig();
            
            serializationConfig.SetDefaultSerializers(
                KafkaSerializerFactory.CreateSerializer<TKey>(config.KeySerializationType),
                KafkaSerializerFactory.CreateSerializer<TValue>(config.ValueSerializationType));
            
            return serializationConfig;
        }

        public void Produce(
            string topic,
            TValue value,
            DateTime? timestamp = null)
        {
            DateTime actualTimestamp = timestamp ?? DateTime.Now;
            
            Produce(
                topic,
                default,
                value,
                actualTimestamp);
        }


        public void Produce(
            string topic,
            TKey key,
            TValue value,
            DateTime? timestamp = null)
        {
            DateTime actualTimestamp = timestamp ?? DateTime.Now;

            _logger.LogInformation(
                "Producing message with key = {} and value = {}, in topic {} at timestamp {}",
                key,
                value,
                topic,
                actualTimestamp);
            
            _cluster.Produce(
                topic,
                key,
                value,
                actualTimestamp);
        }
    }
}