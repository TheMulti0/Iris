using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
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
                },
                {
                    Route.Language,
                    new[]
                    {
                        "/language"
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
                Route? lastRoute = GetRoute(update);
                if (lastRoute == null)
                {
                    return;
                }

                Context context = await CreateContext(update, updates);
                while (lastRoute != null)
                {
                    IRedirectResult result = await ExecuteCommand(
                        lastRoute,
                        context,
                        token);

                    lastRoute = result.Route;
                    context = result.Context ?? context;
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

            ICommand command = _commandFactory.Create(type, context);

            return command.ExecuteAsync(token);
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

            IObservable<Update> chatUpdates = updates
                .Where(u => u.GetChatId().GetHashCode() == contextChatId.GetHashCode());
            
            Task<Update> nextMessage = chatUpdates
                .Where(u => u.Type == UpdateType.Message)
                .FirstOrDefaultAsync(u => GetRoute(u) == null)
                .ToTask();

            Task<Update> nextCallbackQuery = chatUpdates
                .Where(u => u.Type == UpdateType.CallbackQuery)
                .FirstOrDefaultAsync()
                .ToTask();

            Connection connection = await _connectionsRepository.GetAsync(update.GetUser());

            return new Context(
                _client,
                nextMessage,
                nextCallbackQuery,
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
                    return typeof(SettingsCommand);
                
                case Route.Users:
                    return typeof(UsersCommand);
                
                case Route.User:
                    return typeof(UserCommand);

                case Route.AddUser:
                    return typeof(AddUserCommand);
                
                case Route.RemoveUser:
                    return typeof(RemoveUserCommand);
                
                case Route.SelectPlatform:
                    return typeof(SelectPlatformCommand);

                case Route.Connect:
                    return typeof(ConnectCommand);
                
                case Route.Disconnect:
                    return typeof(DisconnectCommand);
                
                case Route.Connection:
                    return typeof(ConnectionCommand);
                
                case Route.Language:
                    return typeof(LanguageCommand);

                case Route.SetUserDisplayName:
                    return typeof(SetUserDisplayNameCommand);
                
                case Route.SetUserLanguage:
                    return typeof(SetUserLanguageCommand);
                
                case Route.ToggleUserPrefix:
                    return typeof(ToggleUserPrefixCommand);
                
                case Route.ToggleUserSuffix:
                    return typeof(ToggleUserSuffixCommand);
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(route), route, null);
            }
        }
    }
}