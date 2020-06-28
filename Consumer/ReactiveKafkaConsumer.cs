using System;
using Confluent.Kafka;

namespace Consumer
{
    public class ReactiveKafkaConsumer<TKey, TValue> : IDisposable
    {
        private readonly IConsumer<TKey, TValue> _consumer;

        public ReactiveKafkaConsumer(ConsumerConfig config)
        {
            _consumer = new ConsumerBuilder<TKey, TValue>(config)
                .SetValueDeserializer(new JsonDeserializer<TValue>())
                .Build();
        }

        public void Dispose() 
            => _consumer?.Dispose();
    }
}