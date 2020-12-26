using System.Threading;
using System.Threading.Tasks;
using Common;
using Extensions;
using Microsoft.Extensions.Logging;

namespace PollRulesManager
{
    public class ChatPollRequestsConsumer : IConsumer<ChatPollRequest>
    {
        private readonly IProducer<ChatPollRequest> _producer;
        private readonly ILogger<ChatPollRequestsConsumer> _logger;

        public ChatPollRequestsConsumer(
            IProducer<ChatPollRequest> producer,
            ILogger<ChatPollRequestsConsumer> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public async Task ConsumeAsync(ChatPollRequest request, CancellationToken token)
        {
            _logger.LogInformation("Received chat poll request {}", request);

            _producer.Send(request);
        }
    }
}