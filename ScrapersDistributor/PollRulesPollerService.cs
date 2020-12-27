using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Extensions;
using Microsoft.Extensions.Hosting;

namespace ScrapersDistributor
{
    public class PollRulesPollerService : BackgroundService
    {
        private readonly IPollRulesManagerClient _client;
        private readonly IConsumer<PollRequest> _consumer;

        public PollRulesPollerService(
            IPollRulesManagerClient client,
            IConsumer<PollRequest> consumer)
        {
            _client = client;
            _consumer = consumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            List<UserPollRule> userPollRules = await _client.Get(stoppingToken);
            foreach (UserPollRule rule in userPollRules)
            {
                await _consumer.ConsumeAsync(
                    new PollRequest(Request.StartPoll, rule),
                    stoppingToken);
            }
        }
    }
}