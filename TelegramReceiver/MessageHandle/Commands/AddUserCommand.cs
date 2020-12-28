using System;
using System.Linq;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Common;
using Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramReceiver.Data;
using UserDataLayer;
using Message = Telegram.Bot.Types.Message;
using Update = Telegram.Bot.Types.Update;
using User = Common.User;
using UpdateType = Telegram.Bot.Types.Enums.UpdateType;

namespace TelegramReceiver
{
    
    internal class AddUserCommand : ICommand
    {
        private readonly IConnectionsRepository _connectionsRepository;
        private readonly ISavedUsersRepository _savedUsersRepository;
        private readonly IProducer<ChatPollRequest> _producer;
        private readonly TimeSpan _defaultInterval;

        public const string CallbackPath = "platform";

        public ITrigger[] Triggers { get; } = {
            new StartsWithCallbackTrigger(CallbackPath),
        };

        public AddUserCommand(
            TelegramConfig config,
            IConnectionsRepository connectionsRepository,
            ISavedUsersRepository savedUsersRepository,
            IProducer<ChatPollRequest> producer)
        {
            _connectionsRepository = connectionsRepository;
            _savedUsersRepository = savedUsersRepository;
            _producer = producer;
            
            _defaultInterval = config.DefaultInterval;
        }

        public async Task OperateAsync(Context context)
        {
            (ITelegramBotClient client, IObservable<Update> incoming, Update currentUpdate) = context;
            CallbackQuery query = currentUpdate.CallbackQuery;

            Platform platform = GetPlatform(query);

            await SendRequestMessage(client, query, platform);

            // Wait for the user to reply with desired user id

            Update newUpdate = await incoming.FirstAsync(update => update.Type == UpdateType.Message);
            
            await AddUser(client, newUpdate.Message, platform);
        }

        private static Platform GetPlatform(CallbackQuery query)
        {
            return Enum.Parse<Platform>(query.Data
                .Split("-")
                .Last());
        }

        private static Task SendRequestMessage(
            ITelegramBotClient client,
            CallbackQuery callbackQuery,
            Platform platform)
        {
            Message message = callbackQuery.Message;

            return client.EditMessageTextAsync(
                chatId: message.Chat.Id,
                messageId: message.MessageId,
                text: $"Enter user from {Enum.GetName(platform)}");
        }
        
        private async Task AddUser(ITelegramBotClient client, Message message, Platform platform)
        {
            ChatId contextChat = message.Chat.Id;
            ChatId connectedChat = await _connectionsRepository.GetAsync(message.From) ?? contextChat;;
            string messageText = message.Text;
            
            var user = new User(messageText, platform);
            TimeSpan interval = _defaultInterval;
            
            var userPollRule = new UserPollRule(user, interval);

            _producer.Send(
                new ChatPollRequest(
                    Request.StartPoll,
                    userPollRule,
                    connectedChat));

            await _savedUsersRepository.AddOrUpdateAsync(
                user,
                new UserChatInfo
                {
                    ChatId = connectedChat,
                    Interval = interval
                });

            await client.SendTextMessageAsync(
                chatId: contextChat,
                text: $"Added {messageText} from platform {platform} with max delay set to up to {interval * 2}",
                replyToMessageId: message.MessageId);
        }
    }
}