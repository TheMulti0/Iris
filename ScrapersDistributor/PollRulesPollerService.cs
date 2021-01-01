using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ScrapersDistributor
{
    public class PollRulesPollerService : BackgroundService
    {
        private readonly IPollRulesManagerClient _client;
        private readonly IConsumer<PollRequest> _consumer;
        private readonly ILogger<PollRulesPollerService> _logger;

        public PollRulesPollerService(
            IPollRulesManagerClient client,
            IConsumer<PollRequest> consumer,
            ILogger<PollRulesPollerService> logger)
        {
            _client = client;
            _consumer = consumer;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Polling poll rules");
            
            try
            {
                List<UserPollRule> userPollRules = await _client.Get(stoppingToken);
            
                foreach (UserPollRule rule in userPollRules)
                {
                    await _consumer.ConsumeAsync(
                        new PollRequest(Request.StartPoll, rule),
                        stoppingToken);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to poll poll rules");
            }
            
        }
    }
}