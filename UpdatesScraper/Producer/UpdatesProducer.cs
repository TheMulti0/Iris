using System.Text.Json;
using Common;
using Extensions;
using Microsoft.Extensions.Logging;

namespace UpdatesScraper
{
    internal class UpdatesProducer : IProducer<Update>
    {
        private readonly RabbitMqPublisher _publisher;
        private readonly RabbitMqConfig _config;
        private readonly ILogger<UpdatesProducer> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public UpdatesProducer(
            RabbitMqConfig config,
            ILogger<UpdatesProducer> logger)
        {
            _publisher = new RabbitMqPublisher(config);
            _config = config;
            _logger = logger;

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                Converters = { new MediaJsonConverter() }
            };
        }

        public void Send(Update update)
        {
            _logger.LogInformation("Sending update {}", update);

            _publisher.Publish(
                _config.Destination,
                JsonSerializer.SerializeToUtf8Bytes(update, _jsonSerializerOptions));
        }
    }
}