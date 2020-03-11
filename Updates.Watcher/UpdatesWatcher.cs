using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Updates.Api;

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
            IUpdatesValidator validator,
            TimeSpan interval,
            IUpdatesProvider provider,
            IEnumerable<long> watchedUsersIds)
        {
            _validator = validator;
            _interval = interval;
            _provider = provider;
            _watchedUsersIds = watchedUsersIds;
            _updates = new Subject<IUpdate>();

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