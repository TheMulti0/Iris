using System;
using Confluent.Kafka;

namespace Consumer
{
    public class KafkaConsumer : IDisposable
    {
        private readonly IConsumer<Ignore, Update> _consumer;

        public KafkaConsumer(ConsumerConfig config)
        {
            _consumer = new ConsumerBuilder<Ignore, Update>(config)
                .SetValueDeserializer(new JsonDeserializer<Update>())
                .Build();
        }

        public void Dispose() 
            => _consumer?.Dispose();
    }
}