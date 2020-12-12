using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Extensions;
using Kafka.Public;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;

namespace UpdatesConsumer
{
    public class UpdatesConsumerService : BackgroundService
    {
        private readonly RabbitMqConsumer _updatesConsumer;
        private readonly IUpdateConsumer _consumer;
        private readonly ILogger<UpdatesConsumerService> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private IDisposable _updateSubscription;

        public UpdatesConsumerService(
            RabbitMqConsumer updatesConsumer, 
            IUpdateConsumer consumer,
            ILogger<UpdatesConsumerService> logger)
        {
            _updatesConsumer = updatesConsumer;
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
            _updateSubscription = _updatesConsumer.Messages.SubscribeAsync(OnNext);

            // Dispose the update subscription when service is stopped
            stoppingToken.Register(() => _updateSubscription?.Dispose());

            return Task.CompletedTask;
        }

        private async Task OnNext(BasicDeliverEventArgs record)
        {
            try
            {
                var update = JsonSerializer.Deserialize<Update>(record.Body.Span, _jsonSerializerOptions);
                await _consumer.OnUpdateAsync(update);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "");
            }
        }
    }
}