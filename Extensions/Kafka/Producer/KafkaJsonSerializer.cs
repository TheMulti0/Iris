using System;
using System.IO;
using System.Text.Json;
using Kafka.Public;

namespace Extensions
{
    internal class KafkaJsonSerializer<T> : ISerializer
    {
        private static readonly JsonSerializerOptions Options = CreateJsonSerializerOptions();
        private static readonly StringSerializer StringSerializer = new StringSerializer();

        public int Serialize(object input, MemoryStream toStream)
        {
            var json = JsonSerializer.Serialize(input, Options);

            return StringSerializer.Serialize(json, toStream);
        }

        private static JsonSerializerOptions CreateJsonSerializerOptions()
        {
            return new JsonSerializerOptions
            {
                Converters =
                {
                    new DateTimeConverter()
                }
            };
        }
    }
}