using System;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Extensions
{
    public class RabbitMqConsumer : IDisposable
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly CancellationTokenSource _cts = new();
        private readonly RabbitMqConfig _config;
        private readonly Func<BasicDeliverEventArgs, Task> _onMessage;

        public RabbitMqConsumer(
            RabbitMqConfig config,
            Func<BasicDeliverEventArgs, Task> onMessage)
        {
            _config = config;
            _onMessage = onMessage;

            Connect(config);
        }

        private void Connect(RabbitMqConfig config)
        {
            var factory = new ConnectionFactory
            {
                Uri = config.ConnectionString
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (_, message) =>
            {
                Task.Run(() => Consume(message), _cts.Token);
            };

            _channel.BasicConsume(config.Destination, false, consumer);
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
            
            _channel.Close();
            _channel?.Dispose();
            
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}