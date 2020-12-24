using System.Text.Json;
using Common;
using Extensions;

namespace TelegramReceiver
{
    public class ChatPollRequestsProducer : IChatPollRequestsProducer
    {
        private readonly RabbitMqConfig _config;
        private readonly RabbitMqPublisher _publisher;

        public ChatPollRequestsProducer(RabbitMqConfig config)
        {
            _config = config;
            _publisher = new RabbitMqPublisher(config);
        }

        public void SendRequest(ChatPollRequest request)
        {
            byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(request);
            
            _publisher.Publish(_config.Destination, bytes);
        }
    }
}