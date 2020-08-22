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
        private readonly IKafkaConsumer<string, Update> _updateConsumer;
        private readonly TelegramBot _bot;
        private readonly ILogger<UpdateConsumer> _logger;
        private IDisposable _updateSubscription;

        public UpdateConsumer(
            IKafkaConsumer<string, Update> updateConsumer, 
            TelegramBot bot,
            ILogger<UpdateConsumer> logger)
        {
            _updateConsumer = updateConsumer;
            _bot = bot;
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
                await _bot.SendAsync(record.Value, record.Key);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Sending failed {} {}", e.Message, e.StackTrace);
            }
        }
    }
}