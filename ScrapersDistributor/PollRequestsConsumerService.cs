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

namespace ScrapersDistributor
{
    public class PollRequestsConsumerService : BackgroundService
    {
        private readonly RabbitMqConfig _config;
        private readonly IPollRequestsConsumer _requestsConsumer;
        private readonly ILogger<PollRequestsConsumerService> _logger;

        private RabbitMqConsumer _consumer;

        public PollRequestsConsumerService(
            RabbitMqConfig config, 
            IPollRequestsConsumer requestsConsumer,
            ILogger<PollRequestsConsumerService> logger)
        {
            _config = config;
            _requestsConsumer = requestsConsumer;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer = new RabbitMqConsumer(_config, OnMessage(stoppingToken));
            
            // TODO Get current jobs from REST
            
            // Dispose the consumer when service is stopped
            stoppingToken.Register(() => _consumer.Dispose());

            return Task.CompletedTask;
        }

        private Func<BasicDeliverEventArgs, Task> OnMessage(CancellationToken token)
        {
            return async message =>
            {
                try
                {
                    string json = Encoding.UTF8.GetString(message.Body.Span.ToArray());
                    
                    var request = JsonSerializer.Deserialize<PollRequest>(json)
                                 ?? throw new NullReferenceException($"Failed to deserialize {json}");

                    await _requestsConsumer.OnRequestAsync(request, token);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "");
                }
            };
        }
    }
}