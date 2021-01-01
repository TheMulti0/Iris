using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Extensions;
using Microsoft.Extensions.Logging;

namespace ScrapersDistributor
{
    internal class PollRequestsConsumer : IConsumer<PollRequest>
    {
        private record RunningOperation(
            Task Task,
            CancellationTokenSource TokenSource);
        
        private readonly IProducer<User> _producer;
        private readonly ConcurrentDictionary<UserPollRule, RunningOperation> _userPollOperations = new();
        private readonly ILogger<PollRequestsConsumer> _logger;

        public PollRequestsConsumer(
            IProducer<User> producer,
            ILogger<PollRequestsConsumer> logger)
        {
            _producer = producer;
            _logger = logger;
        }
        
        public Task ConsumeAsync(PollRequest request, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("Received poll request {}", request);
                
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
            _logger.LogInformation("Adding user poll rule {}", rule);

            var cts = new CancellationTokenSource();
            
            var userTask = Task.Factory.StartNew(
                () => PeriodicallySendJobs(rule, cts.Token),
                TaskCreationOptions.AttachedToParent);

            var operation = new RunningOperation(
                userTask,
                cts);

            _userPollOperations.AddOrUpdate(
                rule,
                _ => operation,
                (_, _) => operation);
        }

        private void RemoveUserPollRule(UserPollRule rule)
        {
            _logger.LogInformation("Removing user poll rule {}", rule);
            
            _userPollOperations.TryRemove(rule, out RunningOperation operation);
            
            operation?.TokenSource.Cancel();
        }

        private async Task PeriodicallySendJobs(
            UserPollRule rule,
            CancellationToken token)
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
                    _producer.Send(user);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to send job for {}", rule);
                }
                
                await Task.Delay(interval, token);
            }
        }
    }
}