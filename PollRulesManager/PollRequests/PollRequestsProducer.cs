using System.Text.Json;
using Common;
using Extensions;

namespace PollRulesManager
{
    public class PollRequestsProducer : IPollRequestsProducer
    {
        private readonly RabbitMqConfig _config;
        private readonly RabbitMqPublisher _publisher;

        public PollRequestsProducer(RabbitMqConfig config)
        {
            _config = config;
            _publisher = new RabbitMqPublisher(config);
        }

        public void SendPollRequest(PollRequest request)
        {
            byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(request);
            
            _publisher.Publish(_config.Destination, bytes);
        }
    }
}