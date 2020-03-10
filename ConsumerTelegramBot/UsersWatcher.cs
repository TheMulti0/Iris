using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using ProducerApi;

namespace ConsumerTelegramBot
{
    internal class UsersWatcher : IUsersWatcher
    {
        private readonly TimeSpan _interval;
        private readonly IProducer _producer;
        private readonly IEnumerable<long> _watchedUsersIds;
        private readonly Subject<IUpdate> _updates;

        public IObservable<IUpdate> Updates => _updates;

        public UsersWatcher(
            TimeSpan interval,
            IProducer producer,
            IEnumerable<long> watchedUsersIds)
        {
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
            foreach (long userId in _watchedUsersIds)
            {
                IEnumerable<IUpdate> updates = await _producer.GetUpdates(userId);

                foreach (IUpdate update in updates)
                {
                    _updates.OnNext(update);
                }
            }
        }
    }
}