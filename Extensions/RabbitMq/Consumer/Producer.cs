using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Extensions
{
    public class Producer<T> : IProducer<T>
    {
        private readonly RabbitMqConfig _config;
        private readonly RabbitMqPublisher _publisher;
        private readonly ILogger<Producer<T>> _logger;

        public Producer(
            RabbitMqConfig config,
            ILogger<Producer<T>> logger)
        {
            _config = config;
            _publisher = new RabbitMqPublisher(config);
            _logger = logger;
        }

        public void Send(T item)
        {
            _logger.LogInformation("Sending {}", item);
            
            byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(item);
            
            _publisher.Publish(_config.Destination, bytes);
        }
    }
}