using Kafka.Public;

namespace Extensions
{
    internal static class KafkaSerializerFactory
    {
        public static ISerializer CreateSerializer<T>(SerializationType type)
        {
            return type switch 
            {
                SerializationType.Json => new KafkaJsonSerializer<T>(),
                _ => new StringSerializer()
                };
        }
    }
}