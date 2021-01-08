using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramReceiver.Data;
using Update = Telegram.Bot.Types.Update;

namespace TelegramReceiver
{
    public class CommandExecutor
    {
        private readonly ITelegramBotClient _client;
        private readonly IEnumerable<ICommand> _commands;
        private readonly IConnectionsRepository _connectionsRepository;
        private readonly Languages _languages;
        private readonly ILogger<CommandExecutor> _logger;

        public CommandExecutor(
            TelegramConfig config,
            IEnumerable<ICommand> commands,
            IConnectionsRepository connectionsRepository,
            Languages languages,
            ILogger<CommandExecutor> logger)
        {
            _client = new TelegramBotClient(config.AccessToken);
            _commands = commands;
            _connectionsRepository = connectionsRepository;
            _languages = languages;
            _logger = logger;
        }

        public async Task ProcessUpdate(
            Update update,
            IObservable<Update> updates,
            CancellationToken token)
        {
            Task<Context> context = CreateContext(update, updates);

            foreach (ICommand command in _commands)
            {
                bool shouldTrigger = command.Triggers
                    .Any(trigger => trigger.ShouldTrigger(update));

                if (!shouldTrigger)
                {
                    continue;
                }
                
                ExecuteCommand(command, await context);
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

            return new Context(
                _client,
                incomingUpdatesFromChat,
                update,
                contextChatId,
                connection?.Chat ?? contextChatId,
                connection?.Language ?? Language.English,
                _languages.Dictionary[connection?.Language ?? Language.English]);
        }

        private void ExecuteCommand(ICommand command, Context context)
        {
            try
            {
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
}