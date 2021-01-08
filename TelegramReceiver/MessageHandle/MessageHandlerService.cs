using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using TelegramReceiver.Data;
using Update = Telegram.Bot.Types.Update;

namespace TelegramReceiver
{
    public class MessageHandlerService : BackgroundService
    {
        private readonly ITelegramBotClient _client;
        private readonly CommandExecutor _commandExecutor;
        private readonly IEnumerable<ICommand> _commands;
        private readonly IConnectionsRepository _connectionsRepository;
        private readonly Languages _languages;
        private readonly ILogger<MessageHandlerService> _logger;

        public MessageHandlerService(
            TelegramConfig config,
            CommandExecutor commandExecutor,
            IEnumerable<ICommand> commands,
            IConnectionsRepository connectionsRepository,
            Languages languages,
            ILogger<MessageHandlerService> logger)
        {
            _client = new TelegramBotClient(config.AccessToken);
            _commandExecutor = commandExecutor;
            _commands = commands;
            _connectionsRepository = connectionsRepository;
            _languages = languages;
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
                await _commandExecutor.ProcessUpdate(update, updates, token);
            }
        }

        private async Task OnUpdate(
            Update update,
            IObservable<Update> updates)
        {
            Context context = null;

            foreach (ICommand command in _commands)
            {
                bool shouldTrigger = command.Triggers
                    .Any(trigger => trigger.ShouldTrigger(update));

                if (!shouldTrigger)
                {
                    continue;
                }
                
                try
                {
                    if (context == null)
                    {
                        context = await CreateContext(update, updates);
                    }

                    Task.Factory.StartNew( 
                        () => command.OperateAsync(context),
                        TaskCreationOptions.AttachedToParent);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to execute command");
                }
            }
        }

        private async Task<Context> CreateContext(
            Update update,
            IObservable<Update> updates)
        {
            ChatId contextChatId = update.GetChatId();

            IObservable<Update> incomingUpdatesFromChat = updates
                .Where(
                    u => u.GetChatId()
                        .GetHashCode() == contextChatId.GetHashCode());

            Connection connection = await _connectionsRepository.GetAsync(update.GetUser());

            var context = new Context(
                _client,
                incomingUpdatesFromChat,
                update,
                contextChatId,
                connection?.Chat ?? contextChatId,
                connection?.Language ?? Language.English,
                _languages.Dictionary[connection?.Language ?? Language.English]);
            return context;
        }

        public override void Dispose()
        {
            _client.StopReceiving();
        }
    }
}