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
using Update = Telegram.Bot.Types.Update;
using User = Telegram.Bot.Types.User;

namespace TelegramReceiver
{
    public class CommandExecutor
    {
        private readonly ITelegramBotClient _client;
        private readonly TelegramConfig _config;
        private readonly CommandFactory _commandFactory;
        private readonly IConnectionsRepository _connectionsRepository;
        private readonly Languages _languages;
        private readonly ILogger<CommandExecutor> _logger;
        
        private static readonly Dictionary<Route?, string> CallbackQueryRoutes;
        private static readonly Dictionary<Route?, string[]> CommandRoutes;
        private static readonly TimeSpan ReactionTimeout = TimeSpan.FromMinutes(5);

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
                    Route.Start,
                    new []
                    {
                        "/start"
                    }
                },
                {
                    Route.Settings,
                    new[]
                    {
                        "/settings",
                        "/manage"
                    }
                },
                {
                    Route.Language,
                    new[]
                    {
                        "/language"
                    }
                },
                {
                    Route.Platforms,
                    new[]
                    {
                        "/subscriptions"
                    }
                },
                {
                    Route.Connect,
                    new[]
                    {
                        "/connect"
                    }
                },
                {
                    Route.Connection,
                    new[]
                    {
                        "/connection"
                    }
                },
                {
                    Route.Disconnect,
                    new[]
                    {
                        "/disconnect"
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
            _config = config;
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
            try
            {
                switch (update.Type)
                {
                    case UpdateType.CallbackQuery:
                    
                        return CallbackQueryRoutes
                            .First(
                                pair => update.CallbackQuery.Data.StartsWith(pair.Value))
                            .Key;

                    case UpdateType.Message:
                    
                        return CommandRoutes.First(
                            pair => pair.Value
                                .Contains(
                                    update.Message.Text.Split(' ').FirstOrDefault()))
                            .Key;
                }
            }
            catch
            {
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

            User user = update.GetUser();
            Connection connection = await GetConnectionAsync(user, contextChatId);

            return new Context(
                _client,
                GetNextMessage(chatUpdates),
                GetNextCallbackQuery(chatUpdates),
                update,
                contextChatId,
                connection?.Chat ?? contextChatId,
                connection?.Language ?? Language.English,
                _languages.Dictionary[connection?.Language ?? Language.English],
                _config.SuperUsers.Contains(user?.Username));
        }

        private async Task<Connection> GetConnectionAsync(User user, ChatId contextChatId)
        {
            Connection connection = await _connectionsRepository.GetAsync(user);

            if (connection != null)
            {
                return connection;
            }
            
            await _connectionsRepository
                .AddOrUpdateAsync(user, contextChatId, Language.English);

            return new Connection
            {
                User = user,
                Chat = contextChatId,
                Language = Language.English
            };
        }

        private static async Task<Update> GetNextMessage(IObservable<Update> chatUpdates)
        {
            Update nextUpdate = await chatUpdates
                .FirstOrDefaultAsync()
                .Timeout(ReactionTimeout);

            if (nextUpdate != null &&
                nextUpdate.Type == UpdateType.Message &&
                GetRoute(nextUpdate) == null)
            {
                return nextUpdate;
            }

            return null;
        }

        private static async Task<Update> GetNextCallbackQuery(IObservable<Update> chatUpdates)
        {
            Update nextUpdate = await chatUpdates
                .FirstOrDefaultAsync()
                .Timeout(ReactionTimeout);

            if (nextUpdate != null &&
                nextUpdate.Type == UpdateType.CallbackQuery)
            {
                return nextUpdate;
            }

            return null;
        }

        private Type GetCommandType(Route route)
        {
            switch (route)
            {
                case Route.Start:
                    return typeof(StartCommand);
                
                case Route.Settings:
                    return typeof(SettingsCommand);
                
                case Route.Subscriptions:
                    return typeof(SubscriptionsCommand);
                
                case Route.User:
                    return typeof(UserCommand);

                case Route.AddUser:
                    return typeof(AddUserCommand);
                
                case Route.RemoveUser:
                    return typeof(RemoveUserCommand);
                
                case Route.Platforms: 
                    return typeof(PlatformsCommand);

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
                
                case Route.ToggleUserSendScreenshotOnly:
                    return typeof(ToggleUserSendScreenshotOnlyCommand);
                
                default:
                    _logger.LogError("Failed to find correct command for route {}", route);
                    return null;
            }
        }
    }
}