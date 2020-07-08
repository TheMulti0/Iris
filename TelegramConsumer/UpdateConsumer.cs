using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TelegramConsumer
{
    public class UpdateConsumer : IHostedService
    {
        private readonly Consumer<Unit, Update> _updateConsumer;
        private readonly ISender _sender;
        private readonly ILogger<UpdateConsumer> _logger;
        private IDisposable _updateSubscription;

        public UpdateConsumer(
            Consumer<Unit, Update> updateConsumer, 
            ISender sender,
            ILogger<UpdateConsumer> logger)
        {
            _updateConsumer = updateConsumer;
            _sender = sender;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _updateSubscription = _updateConsumer.Messages.Subscribe(
                OnNext,
                exception => _logger.LogError(exception, "Received error"),
                () => _logger.LogInformation("Update stream is completed"));

            // If starting process is cancelled, dispose the update subscription
            cancellationToken.Register(() => _updateSubscription?.Dispose());

            return Task.CompletedTask;
        }

        private void OnNext(Result<Message<Unit, Update>> result)
        {
            _logger.LogInformation("Received result of update {}", result);

            result.DoAsync(
                message => message.Value.DoAsync(_sender.SendAsync));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _updateSubscription?.Dispose();

            return Task.CompletedTask;
        }
    }
}