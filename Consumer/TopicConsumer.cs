using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Consumer
{
    public class TopicConsumer<TKey, TValue> : IDisposable
    {
        private readonly IConsumer<TKey, TValue> _consumer;

        public IObservable<ConsumeResult<TKey, TValue>> Messages { get; }
        
        public TopicConsumer(TopicConsumerConfig config)
        {
            var consumerConfig = new ConsumerConfig
            {
                GroupId = config.GroupId,
                BootstrapServers = config.BootstrapServers
            };
            
            _consumer = new ConsumerBuilder<TKey, TValue>(consumerConfig)
                .SetValueDeserializer(new JsonDeserializer<TValue>())
                .Build();
            
            Messages = CreateTopicConsumer(
                config.Topic,
                TimeSpan.FromSeconds(config.PollIntervalSeconds),
                ThreadPoolScheduler.Instance);
        }

        private IObservable<ConsumeResult<TKey, TValue>> CreateTopicConsumer(
            string topic,
            TimeSpan interval,
            IScheduler scheduler)
        {
            _consumer.Subscribe(topic);
            
            return Observable.Create<ConsumeResult<TKey, TValue>>(
                observer => OnSubscribe(observer, interval, scheduler));
        }

        private IDisposable OnSubscribe(
            IObserver<ConsumeResult<TKey, TValue>> observer,
            TimeSpan interval,
            IScheduler scheduler)
        {
            async Task Work(IScheduler sch, CancellationToken cts)
            {
                while (!cts.IsCancellationRequested)
                {
                    observer.OnNext(_consumer.Consume(cts));

                    await sch.Sleep(interval, cts);
                }
            }

            return scheduler.ScheduleAsync(Work);
        }
        
        public void Dispose()
        {
            _consumer?.Dispose();
        }
    }
}