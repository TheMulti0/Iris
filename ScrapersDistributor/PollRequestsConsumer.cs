using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Logging;

namespace ScrapersDistributor
{
    internal class PollRequestsConsumer : IPollRequestsConsumer
    {
        private readonly IJobsProducer _producer;
        private readonly ConcurrentDictionary<UserPollRule, Task> _userJobsTasks = new();
        private readonly ILogger<PollRequestsConsumer> _logger;

        public PollRequestsConsumer(
            IJobsProducer producer,
            ILogger<PollRequestsConsumer> logger)
        {
            _producer = producer;
            _logger = logger;
        }
        
        public Task OnRequestAsync(PollRequest request, CancellationToken token)
        {
            try
            {
                (Request type, UserPollRule rule) = request;
                
                if (type == Request.StartPoll)
                {
                    AddUserPollRule(rule);
                }
                else
                {
                    RemoveUserPollRule(rule);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "");
            }

            return Task.CompletedTask;
        }

        private void AddUserPollRule(UserPollRule rule)
        {
            var userTask = Task.Factory.StartNew(
                () => PeriodicallySendJobs(rule),
                TaskCreationOptions.AttachedToParent);

            _userJobsTasks.AddOrUpdate(
                rule,
                _ => userTask,
                (_, _) => userTask);
        }

        private void RemoveUserPollRule(UserPollRule rule)
        {
            _userJobsTasks.TryRemove(rule, out Task _);
        }

        private async Task PeriodicallySendJobs(UserPollRule rule)
        {
            (User user, TimeSpan? i) = rule;

            if (i == null)
            {
                return;
            }

            var interval = (TimeSpan) i;
            
            while (true)
            {
                try
                {
                    _producer.SendJob(user);
          
                    await Task.Delay(interval);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to send job for {}", rule);
                }
            }
        }
    }
}