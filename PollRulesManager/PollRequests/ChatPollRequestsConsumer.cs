﻿using System.Threading;
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

        public async Task OnRequestAsync(ChatPollRequest request, CancellationToken token)
        {
            _logger.LogInformation("Received chat poll request {}", request);

            _producer.SendPollRequest(request);
        }
    }
}