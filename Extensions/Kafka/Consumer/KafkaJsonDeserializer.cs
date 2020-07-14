using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kafka.Public;

namespace Extensions
{
    internal class KafkaJsonDeserializer<T> : IDeserializer
    {
        private static readonly JsonSerializerOptions Options = CreateJsonSerializerOptions();
        private static readonly StringDeserializer StringDeserializer = new StringDeserializer();

        public object Deserialize(MemoryStream fromStream, int length)
        {
            try
            {
                var json = StringDeserializer.Deserialize(fromStream, length).ToString();
                return JsonSerializer.Deserialize<T>(
                    json,
                    Options);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static JsonSerializerOptions CreateJsonSerializerOptions()
        {
            return new JsonSerializerOptions
            {
                Converters =
                {
                    new DateTimeConverter(),
                    new JsonStringEnumConverter()
                }
            };
        }
    }
}