using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Extensions
{
    public class RabbitMqConsumer : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _model;

        public RabbitMqConsumer(
            RabbitMqConfig config,
            Func<BasicDeliverEventArgs, Task> onMessage)
        {
            var factory = new ConnectionFactory
            {
                Uri = config.ConnectionString,
                DispatchConsumersAsync = true
            };
            
            _connection = factory.CreateConnection();
            _model = _connection.CreateModel();
            
            var consumer = new AsyncEventingBasicConsumer(_model);

            consumer.Received += async (_, message) =>
            {
                await onMessage(message);

                _model.BasicAck(message.DeliveryTag, false);
            };

            _model.BasicConsume(config.Destination, false, consumer);
        }

        public void Dispose()
        {
            _model.Close();
            _model?.Dispose();
            
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}