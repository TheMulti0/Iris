using System;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using Kafka.Public;
using Microsoft.Extensions.Logging;

namespace Extensions
{
    public class Producer<TKey, TValue> : IDisposable
    {
        private readonly IClusterClient _cluster;

        public Producer(
            BaseKafkaConfig config,
            ILoggerFactory loggerFactory)
        {
            _cluster = ClusterClientFactory.CreateClusterClient(
                config,
                CreateSerializationConfig(),
                loggerFactory);
        }

        public void Dispose() => _cluster?.Dispose();

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

            _cluster.Produce(
                topic,
                key,
                value,
                actualTimestamp);
        }

        private static SerializationConfig CreateSerializationConfig()
        {
            var serializationConfig = new SerializationConfig();
            
            serializationConfig.SetDefaultSerializers(
                new KafkaJsonSerializer<TKey>(), 
                new KafkaJsonSerializer<TValue>());
            
            return serializationConfig;
        }
    }
}