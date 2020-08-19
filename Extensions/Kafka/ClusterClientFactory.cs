using Kafka.Public;
using Microsoft.Extensions.Logging;

namespace Extensions
{
    internal static class ClusterClientFactory
    {
        public static IClusterClient Create(
            BaseKafkaConfig config,
            SerializationConfig serializationConfig,
            ILoggerFactory loggerFactory)
        {
            Configuration clusterConfig = CreateClusterConfig(
                config,
                serializationConfig);
            
            var logger = new KafkaSharpMicrosoftLogger(
                loggerFactory.CreateLogger($"kafka-sharp - {config.DefaultTopic}"));
            
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