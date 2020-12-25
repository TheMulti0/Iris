using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;

namespace TelegramSender
{
    public class MessagesConsumerService : BackgroundService
    {
        private readonly RabbitMqConfig _config;
        private readonly IMessagesConsumer _consumer;
        private readonly ILogger<MessagesConsumerService> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public MessagesConsumerService(
            RabbitMqConfig config, 
            IMessagesConsumer consumer,
            ILogger<MessagesConsumerService> logger)
        {
            _config = config;
            _consumer = consumer;
            _logger = logger;

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                Converters =
                {
                    new MediaJsonConverter()
                }
            };
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
                    
                    var m = JsonSerializer.Deserialize<Message>(json, _jsonSerializerOptions)
                                  ?? throw new NullReferenceException($"Failed to deserialize {json}");

                    await _consumer.OnMessageAsync(m, token);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "");
                }
            };
        }
    }
}