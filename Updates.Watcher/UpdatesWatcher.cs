using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Updates.Api;
using Updates.Configs;

namespace Updates.Watcher
{
    public class UpdatesWatcher : IUpdatesWatcher
    {
        private readonly IUpdatesValidator _validator;
        private readonly TimeSpan _interval;
        private readonly IUpdatesProvider _provider;
        private readonly IEnumerable<long> _watchedUsersIds;
        private readonly Subject<IUpdate> _updates;

        public IObservable<IUpdate> Updates => _updates;

        public UpdatesWatcher(
            IUpdatesProvider provider,
            IProviderConfig config,
            IUpdatesValidator validator)
        {
            _updates = new Subject<IUpdate>();

            _provider = provider;

            _watchedUsersIds = config.WatchedUsersIds;
            _interval = TimeSpan.FromSeconds(config.PollIntervalSeconds);
            
            _validator = validator;

            Task.Run(RepeatWatch);
        }

        private async Task RepeatWatch()
        {
            var delay = new IntervalDelay(_interval);

            while (true)
            {
                await Watch();

                await delay.DelayTillNext();
            }
        }

        private async Task Watch()
        {
            foreach (long authorId in _watchedUsersIds)
            {
                IEnumerable<IUpdate> updates = await _provider.GetUpdates(authorId);

                foreach (IUpdate update in updates)
                {
                    long updateId = update.Id;
                    
                    if (_validator.WasUpdateSent(updateId, authorId))
                    {
                        continue;
                    }
                    
                    _updates.OnNext(update);
                    _validator.UpdateSent(updateId, authorId);
                }
            }
        }
    }
}