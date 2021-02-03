using System;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Extensions;
using Microsoft.Extensions.Logging;
using Update = Common.Update;

namespace UpdatesScraper
{
    public class PollJobsConsumer : IConsumer<PollJob>
    {
        private readonly UpdatesScraper _scraper;
        private readonly IProducer<Update> _producer;
        private readonly IUserLatestUpdateTimesRepository _userLatestUpdateTimesRepository;
        private readonly ILogger<PollJobsConsumer> _logger;

        public PollJobsConsumer(
            UpdatesScraper scraper,
            IProducer<Update> producer,
            IUserLatestUpdateTimesRepository userLatestUpdateTimesRepository,
            ILogger<PollJobsConsumer> logger)
        {
            _scraper = scraper;
            _producer = producer;
            _userLatestUpdateTimesRepository = userLatestUpdateTimesRepository;
            _logger = logger;
        }

        public async Task ConsumeAsync(PollJob pollJob, CancellationToken token)
        {
            _logger.LogInformation("Received poll job {}", pollJob);

            var user = pollJob.User;

            if (pollJob.MinimumEarliestUpdateTime != null)
            {
                var updateTime = await _userLatestUpdateTimesRepository.GetAsync(user);

                long latestTicks = Math.Max(
                    updateTime.LatestUpdateTime.Ticks,
                    (long) pollJob.MinimumEarliestUpdateTime?.Ticks);
                
                await _userLatestUpdateTimesRepository.AddOrUpdateAsync(
                    user,
                    new DateTime(latestTicks));
            }
            
            var foundUpdates = false;
            
            await foreach (Update update in _scraper.ScrapeUser(user, token))
            {
                foundUpdates = true;
                
                _producer.Send(update);
            }

            if (foundUpdates)
            {
                await _userLatestUpdateTimesRepository.AddOrUpdateAsync(user, DateTime.Now);
            }
            else
            {
                _logger.LogInformation("No new updates found for {}", user);
            }
        }
    }
}
