using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;

namespace Extensions
{
    public class ConsumerService<T> : BackgroundService
    {
        private readonly RabbitMqConfig _config;
        private readonly IConsumer<T> _consumer;
        private readonly ILogger<ConsumerService<T>> _logger;

        public ConsumerService(
            RabbitMqConfig config, 
            IConsumer<T> consumer,
            ILogger<ConsumerService<T>> logger)
        {
            _config = config;
            _consumer = consumer;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new RabbitMqConsumer(_config, OnMessage(stoppingToken));

            // Dispose the consumer when service is stopped
            stoppingToken.Register(() => consumer.Dispose());

            return Task.CompletedTask;
        }

        private Func<BasicDeliverEventArgs, Task> OnMessage(CancellationToken token)
        {
            return async message =>
            {
                try
                {
                    string json = Encoding.UTF8.GetString(message.Body.Span.ToArray());
                    
                    var item = JsonSerializer.Deserialize<T>(json)
                               ?? throw new NullReferenceException($"Failed to deserialize {json}");
                    
                    await _consumer.ConsumeAsync(item, token);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "");
                }
            };
        }
    }
}