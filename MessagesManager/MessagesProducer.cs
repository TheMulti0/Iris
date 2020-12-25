using System.Text.Json;
using Common;
using Extensions;
using Microsoft.Extensions.Logging;

namespace MessagesManager
{
    public class MessagesProducer : IMessagesProducer
    {
        private readonly RabbitMqConfig _config;
        private readonly RabbitMqPublisher _publisher;
        private readonly ILogger<MessagesProducer> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public MessagesProducer(
            RabbitMqConfig config,
            ILogger<MessagesProducer> logger)
        {
            _config = config;
            _publisher = new RabbitMqPublisher(config);
            _logger = logger;
            
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                Converters =
                {
                    new MediaJsonConverter()
                }
            };
        }
        
        public void SendMessage(Message message)
        {
            _logger.LogInformation("Sending message {}", message);
            
            byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(message, _jsonSerializerOptions);
            
            _publisher.Publish(_config.Destination, bytes);
        }
    }
}