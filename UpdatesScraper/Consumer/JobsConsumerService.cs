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

namespace UpdatesScraper
{
    public class JobsConsumerService : BackgroundService
    {
        private readonly RabbitMqConfig _config;
        private readonly IJobsConsumer _consumer;
        private readonly ILogger<JobsConsumerService> _logger;

        public JobsConsumerService(
            RabbitMqConfig config, 
            IJobsConsumer consumer,
            ILogger<JobsConsumerService> logger)
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
                    
                    var user = JsonSerializer.Deserialize<User>(json)
                                 ?? throw new NullReferenceException($"Failed to deserialize {json}");
                    
                    await _consumer.OnJobAsync(user, token);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "");
                }
            };
        }
    }
}