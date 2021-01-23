using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<RabbitMqConsumer> _logger;
        private readonly EventingBasicConsumer _consumer;

        public RabbitMqConsumer(
            RabbitMqConsumerConfig config,
            IModel channel,
            Func<BasicDeliverEventArgs, Task> onMessage,
            ILogger<RabbitMqConsumer> logger)
        {
            _config = config;
            _channel = channel;
            _onMessage = onMessage;
            _logger = logger;
            _consumer = new EventingBasicConsumer(_channel);

            _consumer.Received += Received;
            _channel.BasicConsume(config.Queue, false, _consumer);
        }

        private void Received(object _, BasicDeliverEventArgs message)
        {
            Task.Run(() => Consume(message), _cts.Token);
        }

        private async Task Consume(BasicDeliverEventArgs message)
        {
            try
            {
                await _onMessage(message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to consume message");
                
                if (_config.AckOnlyOnSuccess)
                {
                    return;
                }
            }
            
            _channel.BasicAck(message.DeliveryTag, false);
        }

        public void Dispose()
        {
            _consumer.Received -= Received;

            _cts.Cancel();
        }
    }
}