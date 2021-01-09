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
        private static readonly Dictionary<Route?, string[]> CommandRoutes;

        static CommandExecutor()
        {
            CallbackQueryRoutes = Enum
                .GetValues<Route>()
                .ToDictionary(
                    route => route as Route?,
                    route => route.ToString());

            CommandRoutes = new Dictionary<Route?, string[]>
            {
                {
                    Route.Settings,
                    new[]
                    {
                        "/s",
                        "/settings",
                        "/manage"
                    }
                },
                {
                    Route.Users,
                    new[]
                    {
                        "/users"
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
                Route? route = GetRoute(update);
                if (route == null)
                {
                    return;
                }

                Context context = await CreateContext(update, updates);
                while (route != null)
                {
                    IRedirectResult result = await ExecuteCommand(
                        route,
                        context,
                        token);

                    // If the route is back then the previous route will be invoked in the loop
                    if (result.Route != Route.Back)
                    {
                        route = result.Route;
                    }

                    context = result.Context;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to process Telegram update");
            }
        }

        private Task<IRedirectResult> ExecuteCommand(
            Route? route,
            Context context,
            CancellationToken token)
        {
            if (route == null)
            {
                return null;
            }

            Type type = GetCommandType((Route) route);

            ICommandd commandd = _commandFactory.Create(type, context);

            return commandd.ExecuteAsync(token);
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

        private static Type GetCommandType(Route route)
        {
            switch (route)
            {
                case Route.Settings:
                    return typeof(SettingsCommandd);
                
                case Route.Users:
                    return typeof(UsersCommandd);
                
                case Route.User:
                    return typeof(UserCommandd);

                case Route.AddUser:
                    return typeof(AddUserCommandd);
                
                case Route.RemoveUser:
                    return typeof(RemoveUserCommandd);
                
                case Route.SelectPlatform:
                    return typeof(SelectPlatformCommandd);

                case Route.Connect:
                    return typeof(ConnectCommandd);
                
                case Route.Disconnect:
                    return typeof(DisconnectCommandd);
                
                case Route.Connection:
                    return typeof(ConnectionCommandd);
                
                case Route.SetLanguage:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(route), route, null);
            }

            return null;
        }
    }
}