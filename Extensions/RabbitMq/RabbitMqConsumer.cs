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

        private readonly Subject<BasicDeliverEventArgs> _messages = new();
        public IObservable<BasicDeliverEventArgs> Messages => _messages;

        public RabbitMqConsumer(RabbitMqConfig config)
        {
            var factory = new ConnectionFactory
            {
                Uri = config.ConnectionString
            };
            
            _connection = factory.CreateConnection();
            _model = _connection.CreateModel();
            
            var consumer = new EventingBasicConsumer(_model);

            consumer.Received += (_, args) =>
            {
                _messages.OnNext(args);
            };

            _model.BasicConsume(config.Destination, true, consumer);
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