using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Logging;

namespace MessagesManager
{
    public class UpdatesConsumer : IUpdatesConsumer
    {
        private readonly IMessagesProducer _producer;
        private readonly ILogger<IUpdatesConsumer> _logger;

        public UpdatesConsumer(
            IMessagesProducer producer,
            ILogger<IUpdatesConsumer> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public Task OnUpdateAsync(Update update, CancellationToken token)
        {
            _producer.SendMessage(new Message(update, new List<string>()));

            return Task.CompletedTask;
        }
    }
}