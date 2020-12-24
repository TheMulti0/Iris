using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Logging;

namespace PollRulesManager
{
    public class ChatPollRequestsConsumer : IChatPollRequestsConsumer
    {
        private readonly IPollRequestsProducer _producer;
        private readonly ILogger<IChatPollRequestsConsumer> _logger;

        public ChatPollRequestsConsumer(
            IPollRequestsProducer producer,
            ILogger<IChatPollRequestsConsumer> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public Task OnRequestAsync(ChatPollRequest request, CancellationToken token)
        {
            _producer.SendPollRequest(request);

            return Task.CompletedTask;
        }
    }
}