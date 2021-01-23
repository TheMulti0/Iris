using System;
using System.Text;
using RabbitMQ.Client;

namespace Extensions
{
    public class RabbitMqPublisher
    {
        private readonly RabbitMqProducerConfig _config;
        private readonly IModel _channel;

        public RabbitMqPublisher(
            RabbitMqProducerConfig config,
            IModel channel)
        {
            _config = config;
            _channel = channel;
        }
        
        public void Publish(string key, byte[] value)
        {
            Console.WriteLine("\n\n" + Encoding.UTF8.GetString(value) + "\n");

            _channel.BasicPublish(_config.Exchange, key, body: value);
        }
    }
}