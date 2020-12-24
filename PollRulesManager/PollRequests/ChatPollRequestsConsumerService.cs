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

namespace PollRulesManager
{
    public class ChatPollRequestsConsumerService : BackgroundService
    {
        private readonly RabbitMqConfig _config;
        private readonly IChatPollRequestsConsumer _consumer;
        private readonly ILogger<ChatPollRequestsConsumerService> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public ChatPollRequestsConsumerService(
            RabbitMqConfig config, 
            IChatPollRequestsConsumer consumer,
            ILogger<ChatPollRequestsConsumerService> logger)
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
                    
                    var request = JsonSerializer.Deserialize<ChatPollRequest>(json, _jsonSerializerOptions)
                                 ?? throw new NullReferenceException($"Failed to deserialize {json}");
                    
                    await _consumer.OnRequestAsync(request, token);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "");
                }
            };
        }
    }
}