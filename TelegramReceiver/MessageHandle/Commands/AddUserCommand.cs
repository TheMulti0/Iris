using System;
using System.Linq;
using System.Reactive.Linq;
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

        public const string CallbackPath = "platform";

        public ITrigger[] Triggers { get; } = {
            new StartsWithCallbackTrigger(CallbackPath),
        };

        public AddUserCommand(
            IConnectionsRepository connectionsRepository,
            ISavedUsersRepository savedUsersRepository,
            IProducer<ChatPollRequest> producer)
        {
            _connectionsRepository = connectionsRepository;
            _savedUsersRepository = savedUsersRepository;
            _producer = producer;
        }

        public async Task OperateAsync(Context context)
        {
            (ITelegramBotClient client, IObservable<Update> incoming, Update currentUpdate) = context;
            CallbackQuery query = currentUpdate.CallbackQuery;

            string platform = GetPlatform(query);

            await SendRequestMessage(client, query, platform);

            // Wait for the user to reply with desired user id

            Update newUpdate = await incoming.FirstAsync(update => update.Type == UpdateType.Message);
            
            await AddUser(client, newUpdate.Message, platform);
        }

        private static string GetPlatform(CallbackQuery query)
        {
            return query.Data
                .Split("-")
                .Last();
        }

        private static Task SendRequestMessage(
            ITelegramBotClient client,
            CallbackQuery callbackQuery,
            string platform)
        {
            Message message = callbackQuery.Message;

            return client.EditMessageTextAsync(
                chatId: message.Chat.Id,
                messageId: message.MessageId,
                text: $"Enter user from {platform}");
        }
        
        private async Task AddUser(ITelegramBotClient client, Message message, string platform)
        {
            ChatId contextChat = message.Chat.Id;
            ChatId connectedChat = await _connectionsRepository.GetAsync(message.From) ?? contextChat;;
            string messageText = message.Text;
            
            var user = new User(messageText, messageText, platform);
            TimeSpan interval = TimeSpan.FromMinutes(30);
            
            var userPollRule = new UserPollRule(user, interval);

            _producer.Send(
                new ChatPollRequest(
                    Request.StartPoll,
                    userPollRule,
                    connectedChat));

            await _savedUsersRepository.AddOrUpdateAsync(
                user,
                new ChatInfo
                {
                    Chat = connectedChat,
                    Interval = interval
                });

            await client.SendTextMessageAsync(
                chatId: contextChat,
                text: $"Added {messageText} from platform {platform}",
                replyToMessageId: message.MessageId);
        }
    }
}