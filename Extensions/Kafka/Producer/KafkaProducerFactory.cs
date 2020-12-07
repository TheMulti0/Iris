using System.Text.Json;
using Kafka.Public;
using Microsoft.Extensions.Logging;

namespace Extensions
{
    public static class KafkaProducerFactory
    {
        public static IKafkaProducer<TKey, TValue> Create<TKey, TValue>(
            BaseKafkaConfig config,
            ILoggerFactory loggerFactory,
            JsonSerializerOptions options)
            where TKey : class 
            where TValue : class
        {
            SerializationConfig serializationConfig = CreateSerializationConfig(config, options);
            
            IClusterClient clusterClient = ClusterClientFactory.Create(
                config,
                serializationConfig,
                loggerFactory);
            
            return new KafkaProducer<TKey, TValue>(config.Topic, clusterClient);
        }
        
        private static SerializationConfig CreateSerializationConfig(
            BaseKafkaConfig config,
            JsonSerializerOptions options)
        {
            var serializationConfig = new SerializationConfig();
            
            serializationConfig.SetDefaultSerializers(
                KafkaSerializerFactory.CreateSerializer(config.KeySerializationType, options),
                KafkaSerializerFactory.CreateSerializer(config.ValueSerializationType, options));
            
            return serializationConfig;
        }
    }
}