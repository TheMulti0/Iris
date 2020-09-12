using System.Text.Json;
using Kafka.Public;

namespace Extensions
{
    internal static class KafkaSerializerFactory
    {
        public static ISerializer CreateSerializer(SerializationType type, JsonSerializerOptions options)
        {
            return type switch 
            {
                SerializationType.Json => new KafkaJsonSerializer(options),
                _ => new StringSerializer()
            };
        }
    }
}