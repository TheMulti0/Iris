using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Extensions;
using Microsoft.Extensions.Logging;

namespace ScrapersDistributor
{
    internal class SubscriptionsConsumer : IConsumer<SubscriptionRequest>
    {
        private record RunningOperation(
            Task Task,
            CancellationTokenSource TokenSource);
        
        private readonly IProducer<PollJob> _producer;
        private readonly ConcurrentDictionary<Subscription, RunningOperation> _userSubscriptionsOperations = new();
        private readonly ILogger<SubscriptionsConsumer> _logger;

        public SubscriptionsConsumer(
            IProducer<PollJob> producer,
            ILogger<SubscriptionsConsumer> logger)
        {
            _producer = producer;
            _logger = logger;
        }
        
        public Task ConsumeAsync(SubscriptionRequest request, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("Received poll request {}", request);
                
                (SubscriptionType type, Subscription rule) = request;
                
                if (type == SubscriptionType.Subscribe)
                {
                    AddUserSubscription(rule);
                }
                else
                {
                    RemoveUserSubscription(rule);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "");
            }

            return Task.CompletedTask;
        }

        private void AddUserSubscription(Subscription subscription)
        {
            _logger.LogInformation("Adding user subscription {}", subscription);

            var cts = new CancellationTokenSource();
            
            Task userTask = Task.Run(
                () => PeriodicallySendJobs(subscription, cts.Token),
                cts.Token);

            var operation = new RunningOperation(
                userTask,
                cts);

            _userSubscriptionsOperations.AddOrUpdate(
                subscription,
                _ => operation,
                (_, _) => operation);
        }

        private void RemoveUserSubscription(Subscription subscription)
        {
            _logger.LogInformation("Removing user subscription {}", subscription);
            
            _userSubscriptionsOperations.TryRemove(subscription, out RunningOperation operation);
            
            operation?.TokenSource.Cancel();
        }

        private async Task PeriodicallySendJobs(
            Subscription rule,
            CancellationToken token)
        {
            (User user, TimeSpan? interval) = rule;

            if (interval == null)
            {
                return;
            }

            var pollJob = new PollJob(user, DateTime.Now);
            _producer.Send(pollJob, Enum.GetName(user.Platform));

            await SendDelayLoop(
                rule,
                new PollJob(user, null),
                Enum.GetName(user.Platform),
                (TimeSpan) interval,
                token);
        }

        private async Task SendDelayLoop(
            Subscription subscription,
            PollJob pollJob,
            string platformName,
            TimeSpan interval,
            CancellationToken token)
        {
            while (true)
            {
                _logger.LogInformation("Delaying {} for another {}", subscription, interval);
                await Task.Delay(interval, token);

                try
                {
                    _producer.Send(pollJob, platformName);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to send job for {}", subscription);
                }
            }
        }
    }
}