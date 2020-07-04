using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Consumer;
using Microsoft.Extensions.Hosting;

namespace TelegramConsumer
{
    public class UpdateConsumer : IHostedService
    {
        private readonly Consumer<Unit, Update> _updateConsumer;

        public UpdateConsumer(Consumer<Unit, Update> updateConsumer)
        {
            _updateConsumer = updateConsumer;
        }

        public Task StartAsync(CancellationToken cancellationToken) => throw new System.NotImplementedException();

        public Task StopAsync(CancellationToken cancellationToken) => throw new System.NotImplementedException();
    }
}