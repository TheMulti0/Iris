using System;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Extensions;
using Microsoft.Extensions.Logging;
using UserDataLayer;

namespace PollRulesManager
{
    public class ChatPollRequestsConsumer : IConsumer<ChatPollRequest>
    {
        private readonly ISavedUsersRepository _repository;
        private readonly IProducer<ChatPollRequest> _producer;
        private readonly ILogger<ChatPollRequestsConsumer> _logger;

        public ChatPollRequestsConsumer(
            ISavedUsersRepository repository,
            IProducer<PollRequest> producer,
            ILogger<ChatPollRequestsConsumer> logger)
        {
            _repository = repository;
            _producer = producer;
            _logger = logger;
        }

        public async Task ConsumeAsync(ChatPollRequest request, CancellationToken token)
        {
            _logger.LogInformation("Received chat poll request {}", request);

            if (request.Request == Request.StartPoll)
            {
                await _repository.AddOrUpdateAsync(request.PollRule.User, new UserChatInfo
                {
                    ChatId = request.ChatId,
                    Interval = (TimeSpan) request.PollRule.Interval
                });
            }
            else
            {
                await _repository.RemoveAsync(request.PollRule.User, request.ChatId);
            }

            _producer.Send(request);
        }
    }
}