using System.Text.Json;
using Common;
using Extensions;
using Microsoft.Extensions.Logging;

namespace ScrapersDistributor
{
    internal class JobsProducer : IJobsProducer
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

        public void SendJob(User user)
        {
            _logger.LogInformation("Sending job {}", user);
            
            byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(user);
            
            _publisher.Publish(user.Source, bytes);
        }
    }
}