using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace UpdatesProducer
{
    public class UpdatesPollerService : BackgroundService
    {
        private readonly PollerConfig _config;
        private readonly IUpdatesProducer _updatesProducer;
        private readonly IUpdatesProvider _updatesProvider;
        private readonly IUserLatestUpdateTimesRepository _userLatestUpdateTimesRepository;
        private readonly ISentUpdatesRepository _sentUpdatesRepository;
        private readonly ILogger<UpdatesPollerService> _logger;

        public UpdatesPollerService(
            PollerConfig config,
            IUpdatesProducer updatesProducer, 
            IUpdatesProvider updatesProvider,
            IUserLatestUpdateTimesRepository userLatestUpdateTimesRepository,
            ISentUpdatesRepository sentUpdatesRepository,
            ILogger<UpdatesPollerService> logger)
        {
            _config = config;
            _updatesProducer = updatesProducer;
            _updatesProvider = updatesProvider;
            _userLatestUpdateTimesRepository = userLatestUpdateTimesRepository;
            _sentUpdatesRepository = sentUpdatesRepository;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                stoppingToken.ThrowIfCancellationRequested();

                _logger.LogInformation("Beginning to poll all users");
                
                await Poll(stoppingToken);
                
                _logger.LogInformation("Finished polling all users");
                _logger.LogInformation("Sleeping for {}", _config.Interval);

                await Task.Delay(_config.Interval, stoppingToken);
            }
        }

        private async Task Poll(CancellationToken cancellationToken)
        {
            foreach (string userId in _config.WatchedUserIds)
            {
                try
                {
                    await PollUser(userId, cancellationToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to poll updates of {}", userId);
                }
            }
        }

        private async Task PollUser(string userId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Polling {}", userId);
            
            // TODO Download videos

            IEnumerable<Update> updates = (await GetUpdates(userId, cancellationToken))
                .ToList();
            
            foreach (Update update in updates)
            {
                await SendUpdate(update);
            }

            if (updates.Any())
            {
                await _userLatestUpdateTimesRepository.AddOrUpdateAsync(userId, DateTime.Now);
            }
            else
            {
                _logger.LogInformation("No new updates found for {}", userId);
            }

        }

        private async Task SendUpdate(Update update)
        {
            _updatesProducer.Send(update);

            if (_config.StoreSentUpdates)
            {
                await _sentUpdatesRepository.AddAsync(update.Url);
            }
        }

        private async Task<IEnumerable<Update>> GetUpdates(
            string userId, CancellationToken cancellationToken)
        {
            IEnumerable<Update> updates = await _updatesProvider.GetUpdatesAsync(userId);

            UserLatestUpdateTime userLatestUpdateTime = await GetUserLatestUpdateTime(userId);

            return await GetNewUpdates(
                updates, userLatestUpdateTime, cancellationToken);
        }

        private async Task<UserLatestUpdateTime> GetUserLatestUpdateTime(string userId)
        {
            var zero = new UserLatestUpdateTime
            {
                UserId = userId,
                LatestUpdateTime = DateTime.MinValue
            };
            
            return await _userLatestUpdateTimesRepository.GetAsync(userId) ?? zero;
        }

        private async Task<IEnumerable<Update>> GetNewUpdates(
            IEnumerable<Update> updates,
            UserLatestUpdateTime userLatestUpdateTime,
            CancellationToken cancellationToken)
        {
            var newUpdates = updates
                .Where(IsNew(userLatestUpdateTime))
                .OrderBy(update => update.CreationDate);

            if (_config.StoreSentUpdates)
            {
                return await newUpdates.ToAsyncEnumerable()
                    .WhereAwait(NotSent)
                    .ToListAsync(cancellationToken);
            }
            
            return newUpdates;
        }

        private static Func<Update, bool> IsNew(UserLatestUpdateTime userLatestUpdateTime)
        {
            return update => update.CreationDate != null &&
                             update.CreationDate > userLatestUpdateTime.LatestUpdateTime;
        }

        private async ValueTask<bool> NotSent(Update update)
        {
            return await _sentUpdatesRepository.ExistsAsync(update.Url);
        }
    }
}