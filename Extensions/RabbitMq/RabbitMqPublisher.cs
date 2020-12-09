using System;
using RabbitMQ.Client;

namespace Extensions
{
    public class RabbitMqPublisher : IDisposable
    {
        private readonly RabbitMqConfig _config;
        private readonly IConnection _connection;
        private readonly IModel _model;

        public RabbitMqPublisher(RabbitMqConfig config)
        {
            _config = config;
            
            var factory = new ConnectionFactory
            {
                Uri = config.ConnectionString
            };
            
            _connection = factory.CreateConnection();
            _model = _connection.CreateModel();
        }
        
        public void Publish(string key, byte[] value)
        {
            _model.BasicPublish(_config.Destination, key, body: value);
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
            _model?.Dispose();
        }
    }
}