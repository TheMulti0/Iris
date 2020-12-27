using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Args;
using Update = Telegram.Bot.Types.Update;

namespace TelegramReceiver
{
    public class MessageHandlerService : BackgroundService
    {
        private readonly ITelegramBotClient _client;
        private readonly IEnumerable<ICommand> _commands;
        private readonly ILogger<MessageHandlerService> _logger;

        public MessageHandlerService(
            TelegramConfig config,
            IEnumerable<ICommand> commands,
            ILogger<MessageHandlerService> logger)
        {
            _client = new TelegramBotClient(config.AccessToken);
            _commands = commands;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
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
            
            await ListenForUpdates(updates, stoppingToken);
        }

        private async Task ListenForUpdates(
            IObservable<Update> updates,
            CancellationToken token)
        {
            _client.StartReceiving(cancellationToken: token);

            var defaultContext = new Context(_client, updates);

            var asyncEnumerable = updates
                .ToAsyncEnumerable()
                .WithCancellation(token);

            await foreach (Update update in asyncEnumerable)
            {
                await OnUpdate(update, defaultContext with { Update = update});
            }
        }

        private async Task OnUpdate(Update update, Context context)
        {
            foreach (ICommand command in _commands)
            {
                bool shouldTrigger = command.Triggers
                    .Any(trigger => trigger.ShouldTrigger(update));
                
                if (shouldTrigger)
                {
                    await command.OperateAsync(context);
                }
            }
        }

        public override void Dispose()
        {
            _client.StopReceiving();
        }
    }
}