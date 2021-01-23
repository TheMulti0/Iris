using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Extensions
{
    public class ConsumerService<T> : BackgroundService
    {
        private readonly RabbitMqConsumerConfig _config;
        private readonly IModel _channel;
        private readonly IConsumer<T> _consumer;
        private readonly ILogger<ConsumerService<T>> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public ConsumerService(
            RabbitMqConsumerConfig config,
            IModel channel,
            IConsumer<T> consumer,
            ILogger<ConsumerService<T>> logger)
        {
            _config = config;
            _channel = channel;
            _consumer = consumer;
            _logger = logger;
            
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                Converters =
                {
                    new MediaJsonConverter(), new TimeSpanConverter(), new NullableTimeSpanConverter()
                }
            };

        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new RabbitMqConsumer(_config, _channel, OnMessage(stoppingToken));

            // Dispose the consumer when service is stopped
            stoppingToken.Register(() => consumer.Dispose());

            return Task.CompletedTask;
        }

        private Func<BasicDeliverEventArgs, Task> OnMessage(CancellationToken token)
        {
            return async message =>
            {
                string json = "No Json";
                try
                {
                    json = Encoding.UTF8.GetString(message.Body.Span.ToArray());

                    var item = JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions)
                               ?? throw new NullReferenceException($"Failed to deserialize {json}");
                    
                    await _consumer.ConsumeAsync(item, token);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to parse json {}", json);
    
                    throw;
                }
            };
        }
    }
}