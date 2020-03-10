using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using ProducerApi;

namespace ConsumerTelegramBot
{
    internal class UsersWatcher : IUsersWatcher
    {
        private readonly IUpdatesValidator _validator;
        private readonly TimeSpan _interval;
        private readonly IProducer _producer;
        private readonly IEnumerable<long> _watchedUsersIds;
        private readonly Subject<IUpdate> _updates;

        public IObservable<IUpdate> Updates => _updates;

        public UsersWatcher(
            IUpdatesValidator validator,
            TimeSpan interval,
            IProducer producer,
            IEnumerable<long> watchedUsersIds)
        {
            _validator = validator;
            _interval = interval;
            _producer = producer;
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
                IEnumerable<IUpdate> updates = await _producer.GetUpdates(authorId);

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