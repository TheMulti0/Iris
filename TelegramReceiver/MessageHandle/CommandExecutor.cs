using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramReceiver.Data;
using Update = Telegram.Bot.Types.Update;

namespace TelegramReceiver
{
    public class CommandExecutor
    {
        private readonly ITelegramBotClient _client;
        private readonly CommandFactory _commandFactory;
        private readonly IConnectionsRepository _connectionsRepository;
        private readonly Languages _languages;
        private readonly ILogger<CommandExecutor> _logger;
        
        private static readonly Dictionary<Route?, string> CallbackQueryRoutes;
        private static readonly Dictionary<Route, string[]> CommandRoutes;

        static CommandExecutor()
        {
            CallbackQueryRoutes = Enum
                .GetValues<Route>()
                .ToDictionary(
                    route => route as Route?,
                    route => route.ToString());

            CommandRoutes = new Dictionary<Route, string[]>
            {
                {
                    Route.Settings,
                    new[]
                    {
                        "/s",
                        "/settings",
                        "/manage"
                    }
                }
            };
        }

        public CommandExecutor(
            TelegramConfig config,
            CommandFactory commandFactory,
            IConnectionsRepository connectionsRepository,
            Languages languages,
            ILogger<CommandExecutor> logger)
        {
            _client = new TelegramBotClient(config.AccessToken);
            _commandFactory = commandFactory;
            _connectionsRepository = connectionsRepository;
            _languages = languages;
            _logger = logger;
        }

        public async Task ProcessUpdate(
            Update update,
            IObservable<Update> updates,
            CancellationToken token)
        {
            try
            {
                var context = new AsyncLazy<Context>(
                    () => CreateContext(update, updates));

                Route? route = GetRoute(update);

                while (route != null)
                {
                    var newRoute = await ExecuteCommand(
                        route,
                        await context,
                        token);

                    // If the route is not back update to the new route
                    // If the route is back then the previous route will be invoked in the loop
                    if (newRoute != Route.Back)
                    {
                        route = newRoute;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to process Telegram update");
            }
        }

        private async Task<Route?> ExecuteCommand(
            Route? route,
            Context context,
            CancellationToken token)
        {
            if (route == null)
            {
                return null;
            }

            INewCommand newCommand = CreateCommand((Route) route, context);

            var result = await newCommand.ExecuteAsync(token);

            return result.Route;
        }

        private static Route? GetRoute(Update update)
        {
            switch (update.Type)
            {
                case UpdateType.CallbackQuery:
                    
                    return CallbackQueryRoutes
                        .FirstOrDefault(
                            pair => update.CallbackQuery.Data.StartsWith(pair.Value))
                        .Key;

                case UpdateType.Message:
                    
                    return CommandRoutes.FirstOrDefault(
                        pair => pair.Value
                            .Contains(
                                update.Message.Text.Split(' ').FirstOrDefault()))
                        .Key;
            }

            return null;
        }

        private async Task<Context> CreateContext(
            Update update,
            IObservable<Update> updates)
        {
            ChatId contextChatId = update.GetChatId();

            IObservable<Update> incomingUpdatesFromChat = updates
                .Where(
                    u => u.GetChatId().GetHashCode() == contextChatId.GetHashCode());

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

        private INewCommand CreateCommand(Route route, Context context)
        {
            switch (route)
            {
                case Route.Test:
                    return _commandFactory.Create<TestCommand>(context);
                
                case Route.Settings:
                    return _commandFactory.Create<SettingsNewCommand>(context);
                
                case Route.User:
                    return _commandFactory.Create<UserNewCommand>(context);
                
                case Route.Users:
                    return _commandFactory.Create<UsersNewCommand>(context);

                default:
                    throw new ArgumentOutOfRangeException(nameof(route), route, null);
            }
        }
    }
}