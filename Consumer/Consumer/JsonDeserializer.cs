using System;
using System.Text.Json;
using Confluent.Kafka;

namespace Consumer
{
    public class JsonDeserializer<T> : IDeserializer<T>
    {
        private readonly JsonSerializerOptions _options;
        
        public JsonDeserializer()
        {
            _options = new JsonSerializerOptions
            {
                Converters =
                {
                    new DateTimeConverter()
                }
            };
        }
        
        public T Deserialize(
            ReadOnlySpan<byte> data,
            bool isNull,
            SerializationContext context)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(data, _options);
            }
            catch (Exception e)
            {
                Console.WriteLine(e); //TODO use ILogger
                throw;
            }
        }
    }
}