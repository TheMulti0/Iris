using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SubscriptionsDb;
using Telegram.Bot;
using Telegram.Bot.Args;
using Update = Telegram.Bot.Types.Update;

namespace TelegramReceiver
{
    public class MessageHandlerService : BackgroundService
    {
        private readonly ITelegramBotClient _client;
        private readonly CommandExecutor _commandExecutor;
        private readonly IChatSubscriptionsRepository _repository;
        private readonly ISubscriptionsManager _manager;
        private readonly ILogger<MessageHandlerService> _logger;

        public MessageHandlerService(
            TelegramConfig config,
            CommandExecutor commandExecutor,
            IChatSubscriptionsRepository repository,
            ISubscriptionsManager manager,
            ILogger<MessageHandlerService> logger)
        {
            _client = new TelegramBotClient(config.AccessToken);
            _commandExecutor = commandExecutor;
            _repository = repository;
            _manager = manager;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            foreach (SubscriptionEntity subscriptionEntity in _repository.Get())
            {
                TimeSpan minInterval = subscriptionEntity.Chats.Select(chatSubscription => chatSubscription.Interval).Min();
                var subscription = new Subscription(subscriptionEntity.User, minInterval, DateTime.Now);
                
                await _manager.Subscribe(subscription);
            }
            
            var identity = await _client.GetMeAsync(stoppingToken);

            _logger.LogInformation(
                "Logged into Telegram as {} {} (Username = {}, Id = {})",
                identity.FirstName,
                identity.LastName,
                identity.Username,
                identity.Id);
            
            IObservable<Update> updates = Observable.FromEventPattern<UpdateEventArgs>(
                action => _client.OnUpdate += action,
                action => _client.OnUpdate -= action).Select(pattern => pattern.EventArgs.Update);
            
            await ListenToUpdates(updates, stoppingToken);
        }

        private async Task ListenToUpdates(
            IObservable<Update> updates,
            CancellationToken token)
        {
            _client.StartReceiving(cancellationToken: token);

            var asyncEnumerable = updates
                .ToAsyncEnumerable()
                .WithCancellation(token);

            await foreach (Update update in asyncEnumerable)
            {
                Task.Run(() => _commandExecutor.ProcessUpdate(update, updates, token));
            }
        }

        public override void Dispose()
        {
            _client.StopReceiving();
        }
    }
}