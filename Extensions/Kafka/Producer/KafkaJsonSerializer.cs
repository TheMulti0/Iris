using System.IO;
using System.Linq;
using System.Text.Json;
using Kafka.Public;

namespace Extensions
{
    internal class KafkaJsonSerializer : ISerializer
    {
        private static readonly StringSerializer StringSerializer = new StringSerializer();
        private readonly JsonSerializerOptions _options;

        public KafkaJsonSerializer(JsonSerializerOptions options)
        {
            if (options == null)
            {
                options = new JsonSerializerOptions();
            }
            
            _options = options;
            
            if (_options.Converters.All(converter => converter.GetType() != typeof(DateTimeConverter)))
            {
                _options.Converters.Add(new DateTimeConverter());
            }
        }

        public int Serialize(object input, MemoryStream toStream)
        {
            var json = JsonSerializer.Serialize(input, _options);

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