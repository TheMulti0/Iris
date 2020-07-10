using Kafka.Public;

namespace Extensions
{
    internal static class KafkaDeserializerFactory
    {
        public static IDeserializer CreateDeserializer<T>(SerializationType type)
        {
            return type switch 
            {
                SerializationType.Json => new KafkaJsonDeserializer<T>(),
                _ => new StringDeserializer()
            };
        }
    }
}