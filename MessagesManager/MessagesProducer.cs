using System.Text.Json;
using Common;
using Extensions;

namespace MessagesManager
{
    public class MessagesProducer : IMessagesProducer
    {
        private readonly RabbitMqConfig _config;
        private readonly RabbitMqPublisher _publisher;

        public MessagesProducer(RabbitMqConfig config)
        {
            _config = config;
            _publisher = new RabbitMqPublisher(config);
        }
        
        public void SendMessage(Message message)
        {
            byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(message);
            
            _publisher.Publish(_config.Destination, bytes);
        }
    }
}