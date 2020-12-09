using System.Text.Json;
using Common;
using Extensions;
using Microsoft.Extensions.Logging;

namespace UpdatesProducer
{
    internal class RabbitMqUpdatesPublisher : IUpdatesPublisher
    {
        private readonly RabbitMqPublisher _publisher;
        private readonly ILogger<RabbitMqUpdatesPublisher> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public RabbitMqUpdatesPublisher(
            RabbitMqPublisher publisher,
            ILogger<RabbitMqUpdatesPublisher> logger)
        {
            _publisher = publisher;
            _logger = logger;

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                Converters = { new MediaJsonConverter() }
            };
        }

        public void Send(Update update)
        {
            _logger.LogInformation("Sending update {} to RabbitMQ", update);

            _publisher.Publish(
                "update",
                JsonSerializer.SerializeToUtf8Bytes(update, _jsonSerializerOptions));
        }
    }
}