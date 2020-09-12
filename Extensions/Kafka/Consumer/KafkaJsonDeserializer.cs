using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kafka.Public;

namespace Extensions
{
    internal class KafkaJsonDeserializer<T> : IDeserializer
    {
        private static readonly StringDeserializer StringDeserializer = new StringDeserializer();
        private readonly JsonSerializerOptions _options;

        public KafkaJsonDeserializer(JsonSerializerOptions options)
        {
            if (options == null)
            {
                options = new JsonSerializerOptions();
            }
            
            _options = options;
            
            if (_options.Converters.All(
                    converter => converter.GetType() != typeof(DateTimeConverter)))
            {
                _options.Converters.Add(new DateTimeConverter());
            }
            if (_options.Converters.All(
                    converter => converter.GetType() != typeof(JsonStringEnumConverter)))
            {
                _options.Converters.Add(new JsonStringEnumConverter());
            }
        }

        public object Deserialize(MemoryStream fromStream, int length)
        {
            try
            {
                var json = StringDeserializer.Deserialize(fromStream, length).ToString();
                return JsonSerializer.Deserialize<T>(
                    json,
                    _options);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}