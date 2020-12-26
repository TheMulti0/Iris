using System.Text.Json;
using Common;
using Extensions;
using Microsoft.Extensions.Logging;

namespace TelegramReceiver
{
    public class ChatPollRequestsProducer : IProducer<ChatPollRequest>
    {
        private readonly RabbitMqConfig _config;
        private readonly RabbitMqPublisher _publisher;
        private readonly ILogger<ChatPollRequestsProducer> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public ChatPollRequestsProducer(
            RabbitMqConfig config,
            ILogger<ChatPollRequestsProducer> logger)
        {
            _config = config;
            _publisher = new RabbitMqPublisher(config);
            _logger = logger;
            
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                Converters = { new TimeSpanConverter() }
            };
        }

        public void Send(ChatPollRequest request)
        {
            _logger.LogInformation("Sending chat poll request {}", request);
            
            byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(request, _jsonSerializerOptions);
            
            _publisher.Publish(_config.Destination, bytes);
        }
    }
}