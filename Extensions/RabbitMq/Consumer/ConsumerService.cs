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
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<ConsumerService<T>> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public ConsumerService(
            RabbitMqConsumerConfig config,
            IModel channel,
            IConsumer<T> consumer,
            ILoggerFactory loggerFactory)
        {
            _config = config;
            _channel = channel;
            _consumer = consumer;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<ConsumerService<T>>();
            
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
            var consumer = new RabbitMqConsumer(
                _config,
                _channel,
                OnMessage(stoppingToken),
                _loggerFactory.CreateLogger<RabbitMqConsumer>());

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
                    ReadOnlyMemory<byte> readOnlyMemory = message.Body;
                    
                    byte[] bytes = readOnlyMemory.ToArray();

                    json = new UTF8Encoding(false).GetString(bytes);

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
