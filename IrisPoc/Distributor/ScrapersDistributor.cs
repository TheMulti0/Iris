using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace IrisPoc
{
    internal class ScrapersDistributor
    {
        private readonly object _currentLock = new();
        private IEnumerable<UserPollRule> _current;
        
        private readonly Subject<string> _userIds = new();
        public IObservable<string> UserIds => _userIds;

        public ScrapersDistributor(IPollRulesProducer manager)
        {
            lock (_currentLock)
            {
                _current = manager.GetPollRules();
            }

            manager.SetUserPollRules.Subscribe(OnNext);
        }

        private void OnNext(SetPollRule request)
        {
            Console.WriteLine($"Received new job assignment {request}");

            // Not necessary since _current is a reference to the same dictionary
            // UpdateCurrent(assignment);
        }

        private void UpdateCurrent(SetPollRule request)
        {
            (PollRuleType type, UserPollRule userJobInfo, string _) = request;

            lock (_currentLock)
            {
                if (type == PollRuleType.StopPoll)
                {
                    _current = _current.Where(info => info == userJobInfo);
                }
                else
                {
                    _current = _current.Concat(
                        new[]
                        {
                            userJobInfo
                        });
                }
            }
        }

        public void Work()
        {
            try
            {
                Console.WriteLine("Entering distribution loop");

                while (true)
                {
                    IEnumerable<UserPollRule> current;

                    lock (_currentLock)
                    {
                        current = _current;
                    }

                    current
                        .Where(user => user.Interval != null)
                        .GroupBy(user => (TimeSpan) user.Interval)
                        .AsParallel()
                        .ForAll(group => Distribute(group).Wait());
                }
            }
            catch
            {
            }
        }

        private async Task Distribute(IGrouping<TimeSpan, UserPollRule> group)
        {
            Console.WriteLine($"Distributing all users with interval of {group.Key}");
            
            foreach (UserPollRule user in group)
            {
                _userIds.OnNext(user.User.UserId);
            }

            await Task.Delay(group.Key);
        }
    }
}