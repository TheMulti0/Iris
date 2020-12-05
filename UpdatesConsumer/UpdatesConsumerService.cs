using System;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Extensions;
using Kafka.Public;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace UpdatesConsumer
{
    public class UpdatesConsumerService : BackgroundService
    {
        private readonly IKafkaConsumer<string, Update> _updateConsumer;
        private readonly IUpdateConsumer _consumer;
        private readonly ILogger<UpdatesConsumerService> _logger;
        private IDisposable _updateSubscription;

        public UpdatesConsumerService(
            IKafkaConsumer<string, Update> updateConsumer, 
            IUpdateConsumer consumer,
            ILogger<UpdatesConsumerService> logger)
        {
            _updateConsumer = updateConsumer;
            _consumer = consumer;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _updateSubscription = _updateConsumer.Messages.SubscribeAsync(OnNext);

            // Dispose the update subscription when service is stopped
            stoppingToken.Register(() => _updateSubscription?.Dispose());

            return Task.CompletedTask;
        }

        private async Task OnNext(KafkaRecord<string, Update> record)
        {
            try
            {
                await _consumer.OnUpdateAsync(record.Value, record.Key);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "");
            }
        }
    }
}