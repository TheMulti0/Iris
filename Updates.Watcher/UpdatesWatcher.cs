using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Updates.Api;
using Updates.Configs;

namespace Updates.Watcher
{
    public class UpdatesWatcher : IUpdatesWatcher
    {
        private readonly ILogger<IUpdatesWatcher> _logger;
        private readonly IUpdatesProvider _provider;
        private readonly IEnumerable<long> _watchedUsersIds;
        private readonly TimeSpan _interval;
        private readonly Subject<IUpdate> _updates;

        public IObservable<IUpdate> Updates => _updates;

        public UpdatesWatcher(
            ILogger<IUpdatesWatcher> logger,
            IUpdatesProvider provider,
            IProviderConfig config)
        {
            _logger = logger;
            _provider = provider;

            _watchedUsersIds = config.WatchedUsersIds;
            _interval = TimeSpan.FromSeconds(config.PollIntervalSeconds);
            
            _updates = new Subject<IUpdate>();

            Task.Run(RepeatWatch);
            
            _logger.LogInformation("Completed construction");
        }

        private async Task RepeatWatch()
        {
            _logger.LogInformation("Began watch repeat task");
            
            var delay = new IntervalDelay(_interval);

            while (true)
            {
                _logger.LogInformation("Beginning watch");
                await Watch();

                _logger.LogInformation("Beginning sleep");
                await delay.DelayTillNext();
            }
        }

        private async Task Watch()
        {
            foreach (long userId in _watchedUsersIds)
            {
                _logger.LogInformation($"Checking user #{userId}");
                
                IEnumerable<IUpdate> sortedUpdates = await GetUpdates(userId);

                foreach (IUpdate update in sortedUpdates)
                {
                    _logger.LogInformation($"Pushing update #{update.Id}");
                    
                    _updates.OnNext(update);
                }
            }
        }

        private async Task<IEnumerable<IUpdate>> GetUpdates(long userId)
        {
            IEnumerable<IUpdate> updates = await _provider.GetUpdates(userId);

            _logger.LogInformation($"Received unvalidated updates for user #{userId}");

            return updates
                .OrderBy(u => u.CreatedAt);
        }
    }
}