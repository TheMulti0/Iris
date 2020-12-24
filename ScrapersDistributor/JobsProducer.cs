using System.Text.Json;
using Common;
using Extensions;

namespace ScrapersDistributor
{
    internal class JobsProducer : IJobsProducer
    {
        private readonly RabbitMqPublisher _publisher;

        public JobsProducer(RabbitMqConfig config)
        {
            _publisher = new RabbitMqPublisher(config);
        }

        public void SendJob(User user)
        {
            (string userId, string _, string source) = user;
            
            byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(userId);
            
            _publisher.Publish(source, bytes);
        }
    }
}