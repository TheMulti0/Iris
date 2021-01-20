using System;
using System.Text;
using RabbitMQ.Client;

namespace Extensions
{
    public class RabbitMqPublisher : IDisposable
    {
        private readonly RabbitMqConfig _config;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqPublisher(RabbitMqConfig config)
        {
            _config = config;
            
            var factory = new ConnectionFactory
            {
                Uri = config.ConnectionString
            };
            
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }
        
        public void Publish(string key, byte[] value)
        {
            Console.WriteLine("\n\n" + Encoding.UTF8.GetString(value) + "\n");

            _channel.BasicPublish(_config.Destination, key, body: value);
        }

        public void Dispose()
        {
            _channel?.Dispose();

            _connection?.Close();
            _connection?.Dispose();
        }
    }
}