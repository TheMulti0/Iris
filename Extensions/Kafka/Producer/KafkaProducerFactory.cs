using Kafka.Public;
using Microsoft.Extensions.Logging;

namespace Extensions
{
    public static class KafkaProducerFactory
    {
        public static IKafkaProducer<TKey, TValue> Create<TKey, TValue>(
            BaseKafkaConfig config,
            ILoggerFactory loggerFactory)
            where TKey : class 
            where TValue : class
        {
            SerializationConfig serializationConfig = CreateSerializationConfig<TKey, TValue>(config);
            
            IClusterClient clusterClient = ClusterClientFactory.Create(
                config,
                serializationConfig,
                loggerFactory);
            
            return new KafkaProducer<TKey, TValue>(config.Topic, clusterClient);
        }
        
        private static SerializationConfig CreateSerializationConfig<TKey, TValue>(
            BaseKafkaConfig config)
        {
            var serializationConfig = new SerializationConfig();
            
            serializationConfig.SetDefaultSerializers(
                KafkaSerializerFactory.CreateSerializer<TKey>(config.KeySerializationType),
                KafkaSerializerFactory.CreateSerializer<TValue>(config.ValueSerializationType));
            
            return serializationConfig;
        }
    }
}