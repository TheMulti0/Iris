using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Iris.Api;

namespace Iris.Watcher
{
    public class UpdatesWatcher : IUpdatesWatcher
    {
        private readonly ILogger _logger;
        private readonly IUpdatesProvider _provider;
        private readonly User[] _watchedUsers;
        private readonly TimeSpan _interval;
        private readonly Subject<Update> _updates;

        public IObservable<Update> Updates => _updates;

        public UpdatesWatcher(
            ILogger logger,
            IUpdatesProvider provider,
            ProviderConfig config)
        {
            _logger = logger;
            _provider = provider;

            _watchedUsers = config.WatchedUsers;
            _interval = TimeSpan.FromSeconds(config.PollIntervalSeconds);
            
            _updates = new Subject<Update>();

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
            foreach (var user in _watchedUsers)
            {
                _logger.LogInformation($"Checking user #{user.Id}");
                
                IEnumerable<Update> sortedUpdates = await GetUpdates(user);

                foreach (Update update in sortedUpdates)
                {
                    _logger.LogInformation($"Pushing update #{update.Id}");
                    
                    _updates.OnNext(update);
                }
            }
        }

        private async Task<IEnumerable<Update>> GetUpdates(User user)
        {
            try
            {
                IEnumerable<Update> updates = await _provider.GetUpdates(user);

                _logger.LogInformation($"Received unvalidated updates for user #{user.Id}");

                return updates
                    .OrderBy(u => u.CreatedAt);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to GetUpdates\n {}");
                return new List<Update>();
            }
            
        }
    }
}