using System;
using Common;
using Kafka.Public;

namespace UpdatesProducer
{
    internal class KafkaUpdatesProducer : IUpdatesProducer
    {
        private readonly IKafkaProducer<string, Update> _producer;

        public KafkaUpdatesProducer(IKafkaProducer<string, Update> producer)
        {
            _producer = producer;
        }

        public void Send(Update update)
        {
            // TODO service name key

            _producer.Produce(
                key: "Key",
                data: update,
                timestamp: DateTime.Now);
        }
    }
}