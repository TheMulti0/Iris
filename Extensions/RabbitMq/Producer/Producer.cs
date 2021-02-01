using System.Text;
using System.Text.Json;
using Common;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Extensions
{
    public class Producer<T> : IProducer<T>
    {
        private readonly RabbitMqPublisher _publisher;
        private readonly ILogger<Producer<T>> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public Producer(
            RabbitMqProducerConfig config,
            IModel channel,
            ILogger<Producer<T>> logger)
        {
            _publisher = new RabbitMqPublisher(config, channel);
            _logger = logger;
            
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                Converters = { new MediaJsonConverter(), new TimeSpanConverter(), new NullableTimeSpanConverter() }
            };

        }

        public void Send(T item, string routingKey = "")
        {
            _logger.LogInformation("Sending {}", item);

            string json = JsonSerializer.Serialize(item, _jsonSerializerOptions);

            byte[] bytes = new UTF8Encoding(false).GetBytes(json);
            
            _publisher.Publish(routingKey, bytes);
        }
    }
}