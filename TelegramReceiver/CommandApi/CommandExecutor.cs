using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Nito.AsyncEx;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using SubscriptionsDataLayer;
using Update = Telegram.Bot.Types.Update;
using User = Telegram.Bot.Types.User;

namespace TelegramReceiver
{
    public class CommandExecutor
    {
        private readonly ITelegramBotClient _client;
        private readonly TelegramConfig _config;
        private readonly CommandFactory _commandFactory;
        private readonly IChatSubscriptionsRepository _chatSubscriptionsRepository;
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
            IChatSubscriptionsRepository chatSubscriptionsRepository,
            IConnectionsRepository connectionsRepository,
            Languages languages,
            ILogger<CommandExecutor> logger)
        {
            _client = new TelegramBotClient(config.AccessToken);
            _config = config;
            _commandFactory = commandFactory;
            _chatSubscriptionsRepository = chatSubscriptionsRepository;
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
                // if (!context.Connection.HasAgreedToTos && lastRoute != Route.AcceptTos && lastRoute != Route.DeclineTos)
                // {
                //     lastRoute = Route.SendTos;
                // }
                
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
                new AsyncLazy<SubscriptionEntity>(() => GetSavedUser(update)),
                () => GetNextMessage(chatUpdates),
                () => GetNextCallbackQuery(chatUpdates),
                update,
                contextChatId,
                connection,
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

            var connectionProperties = new ConnectionProperties
            {
                Language = Language.English,
                Chat = contextChatId
            };
            
            await _connectionsRepository
                .AddOrUpdateAsync(user, connectionProperties);

            return new Connection(connectionProperties)
            {
                User = user
            };
        }

        private async Task<SubscriptionEntity> GetSavedUser(Update update)
        {
            ObjectId id = GetUserId(update);
            
            if (id == ObjectId.Empty)
            {
                return null;
            }

            SubscriptionEntity entity = await _chatSubscriptionsRepository.GetAsync(id);
            return entity;
        }

        private static ObjectId GetUserId(Update update)
        {
            try
            {
                string[] items = update.CallbackQuery.Data.Split("-");

                return new ObjectId(items[^1]);
            }
            catch
            {
                return ObjectId.Empty;
            }
        }

        private static async Task<Update> GetNextMessage(IObservable<Update> chatUpdates)
        {
            Update nextUpdate;

            try
            {
                nextUpdate = await chatUpdates
                    .FirstOrDefaultAsync()
                    .Timeout(ReactionTimeout);
            }
            catch (TimeoutException)
            {
                nextUpdate = null;
            }

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
                
                case Route.SendTos:
                    return typeof(SendTosCommand);
                
                case Route.AcceptTos:
                    return typeof(AcceptTosCommand);
                
                case Route.DeclineTos:
                    return typeof(DeclineTosCommand);
                
                default:
                    _logger.LogError("Failed to find correct command for route {}", route);
                    return null;
            }
        }
    }
}