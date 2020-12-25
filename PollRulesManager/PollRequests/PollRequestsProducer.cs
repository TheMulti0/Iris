using System.Text.Json;
using Common;
using Extensions;
using Microsoft.Extensions.Logging;

namespace PollRulesManager
{
    public class PollRequestsProducer : IPollRequestsProducer
    {
        private readonly RabbitMqConfig _config;
        private readonly RabbitMqPublisher _publisher;
        private readonly ILogger<PollRequestsProducer> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public PollRequestsProducer(
            RabbitMqConfig config,
            ILogger<PollRequestsProducer> logger)
        {
            _config = config;
            _publisher = new RabbitMqPublisher(config);
            _logger = logger;
            
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                Converters =
                {
                    new TimeSpanConverter()
                }
            };
        }

        public void SendPollRequest(PollRequest request)
        {
            _logger.LogInformation("Sending poll request {}", request);            
            
            byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(request, _jsonSerializerOptions);
            
            _publisher.Publish(_config.Destination, bytes);
        }
    }
}