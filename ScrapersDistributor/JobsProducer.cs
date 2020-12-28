using System;
using System.Text.Json;
using Common;
using Extensions;
using Microsoft.Extensions.Logging;

namespace ScrapersDistributor
{
    internal class JobsProducer : IProducer<User>
    {
        private readonly RabbitMqPublisher _publisher;
        private readonly ILogger<JobsProducer> _logger;

        public JobsProducer(
            RabbitMqConfig config,
            ILogger<JobsProducer> logger)
        {
            _publisher = new RabbitMqPublisher(config);
            _logger = logger;
        }

        public void Send(User user)
        {
            _logger.LogInformation("Sending job {}", user);
            
            byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(user);
            
            _publisher.Publish(
                Enum.GetName(user.Platform),
                bytes);
        }
    }
}