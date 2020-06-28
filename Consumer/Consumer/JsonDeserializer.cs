using System;
using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Consumer
{
    public class JsonDeserializer<T> : IDeserializer<T>
    {
        private readonly ILogger<JsonDeserializer<T>> _logger;
        private readonly JsonSerializerOptions _options;
        
        public JsonDeserializer(ILogger<JsonDeserializer<T>> logger)
        {
            _logger = logger;
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
                _logger.LogError(
                    e,
                    "Failed to deserialize byte data to JSON: {}",
                    Encoding.UTF8.GetString(data));
                throw;
            }
        }
    }
}