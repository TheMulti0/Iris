using Kafka.Public;
using Microsoft.Extensions.Logging;

namespace Extensions
{
    internal static class ClusterClientFactory
    {
        public static IClusterClient CreateClusterClient(
            BaseKafkaConfig config,
            SerializationConfig serializationConfig,
            ILoggerFactory loggerFactory)
        {
            Configuration clusterConfig = CreateClusterConfig(
                config,
                serializationConfig);
            
            var logger = new KafkaSharpMicrosoftLogger(
                loggerFactory.CreateLogger("kafka-sharp"));
            
            return new ClusterClient(
                clusterConfig,
                logger);
        }
        
        private static Configuration CreateClusterConfig(
            BaseKafkaConfig config,
            SerializationConfig serializationConfig)
        {
            return new Configuration
            {
                Seeds = config.BrokersServers,
                SerializationConfig = serializationConfig
            };
        }
    }
}