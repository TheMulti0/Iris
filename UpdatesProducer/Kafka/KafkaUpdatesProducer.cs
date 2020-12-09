using System;
using Common;
using Kafka.Public;
using Microsoft.Extensions.Logging;

namespace UpdatesProducer
{
    internal class KafkaUpdatesProducer : IUpdatesProducer
    {
        private readonly KafkaConfig _config;
        private readonly IKafkaProducer<string, Update> _producer;
        private readonly ILogger<KafkaUpdatesProducer> _logger;

        public KafkaUpdatesProducer(
            KafkaConfig config,
            IKafkaProducer<string, Update> producer,
            ILogger<KafkaUpdatesProducer> logger)
        {
            _config = config;
            _producer = producer;
            _logger = logger;
        }

        public void Send(Update update)
        {
            _logger.LogInformation("Sending update {} to Kafka", update);

            _producer.Produce(
                key: _config.Updates.Key,
                data: update,
                timestamp: DateTime.Now);
        }
    }
}