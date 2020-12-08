using System;
using Common;
using Kafka.Public;
using Microsoft.Extensions.Logging;

namespace UpdatesProducer
{
    internal class KafkaUpdatesProducer : IUpdatesProducer
    {
        private readonly IKafkaProducer<string, Update> _producer;
        private readonly ILogger<KafkaUpdatesProducer> _logger;

        public KafkaUpdatesProducer(
            IKafkaProducer<string, Update> producer,
            ILogger<KafkaUpdatesProducer> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public void Send(Update update)
        {
            _logger.LogInformation("Sending update {} to Kafka", update);

            // TODO service name key

            _producer.Produce(
                key: "Key",
                data: update,
                timestamp: DateTime.Now);
        }
    }
}