using System;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Extensions
{
    public class RabbitMqConsumer : IDisposable
    {
        private readonly CancellationTokenSource _cts = new();
        private readonly RabbitMqConsumerConfig _config;
        private readonly IModel _channel;
        private readonly Func<BasicDeliverEventArgs, Task> _onMessage;

        public RabbitMqConsumer(
            RabbitMqConsumerConfig config,
            IModel channel,
            Func<BasicDeliverEventArgs, Task> onMessage)
        {
            _config = config;
            _channel = channel;
            _onMessage = onMessage;

            Connect(config);
        }

        private void Connect(RabbitMqConsumerConfig config)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (_, message) =>
            {
                Task.Run(() => Consume(message), _cts.Token);
            };

            _channel.BasicConsume(config.Queue, false, consumer);
        }

        private async Task Consume(BasicDeliverEventArgs message)
        {
            try
            {
                await _onMessage(message);
            }
            catch
            {
                if (_config.AckOnlyOnSuccess)
                {
                    return;
                }
            }
            
            _channel.BasicAck(message.DeliveryTag, false);
        }

        public void Dispose()
        {
            _cts.Cancel();
        }
    }
}