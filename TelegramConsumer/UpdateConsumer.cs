using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Extensions;
using Kafka.Public;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TelegramConsumer
{
    public class UpdateConsumer : BackgroundService
    {
        private readonly IKafkaConsumer<Nothing, Update> _updateConsumer;
        private readonly ISender _sender;
        private readonly ILogger<UpdateConsumer> _logger;
        private IDisposable _updateSubscription;

        public UpdateConsumer(
            IKafkaConsumer<Nothing, Update> updateConsumer, 
            ISender sender,
            ILogger<UpdateConsumer> logger)
        {
            _updateConsumer = updateConsumer;
            _sender = sender;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _updateSubscription = _updateConsumer.Messages.SubscribeAsync(OnNext);

            // Dispose the update subscription when service is stopped
            stoppingToken.Register(() => _updateSubscription?.Dispose());

            return Task.CompletedTask;
        }

        private async Task OnNext(KafkaRecord<Nothing, Update> record)
        {
            _logger.LogInformation("Received result of update {}", record);

            try
            {
                await _sender.SendAsync(record.Value);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Sending failed {} {}", e.Message, e.StackTrace);
            }
        }
    }
}