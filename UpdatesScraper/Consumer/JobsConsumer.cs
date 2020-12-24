using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Update = Common.Update;

namespace UpdatesScraper
{
    public class JobsConsumer : IJobsConsumer
    {
        private readonly UpdatesScraper _scraper;
        private readonly IUpdatesProducer _producer;
        private readonly IUserLatestUpdateTimesRepository _userLatestUpdateTimesRepository;
        private readonly ILogger<JobsConsumer> _logger;

        public JobsConsumer(
            UpdatesScraper scraper,
            IUpdatesProducer producer,
            IUserLatestUpdateTimesRepository userLatestUpdateTimesRepository,
            ILogger<JobsConsumer> logger)
        {
            _scraper = scraper;
            _producer = producer;
            _userLatestUpdateTimesRepository = userLatestUpdateTimesRepository;
            _logger = logger;
        }

        public async Task OnJobAsync(string userId, CancellationToken token)
        {
            _logger.LogInformation("Got poll job for {}", userId);

            bool foundUpdates = false;
            
            await foreach (Update update in _scraper.ScrapeUser(userId, token))
            {
                foundUpdates = true;
                
                _producer.SendUpdate(update);
            }

            if (foundUpdates)
            {
                await _userLatestUpdateTimesRepository.AddOrUpdateAsync(userId, DateTime.Now);
            }
            else
            {
                _logger.LogInformation("No new updates found for {}", userId);
            }
        }
    }
}
