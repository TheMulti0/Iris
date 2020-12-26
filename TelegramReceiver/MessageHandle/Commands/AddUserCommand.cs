using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UserDataLayer;
using Message = Telegram.Bot.Types.Message;
using Update = Telegram.Bot.Types.Update;
using User = Common.User;

namespace TelegramReceiver
{
    internal class AddUserCommand : ICommand
    {
        private static readonly ConcurrentDictionary<ChatId, string> ChatPlatforms = new(new ChatIdIEqualityComparer());
        private readonly ISavedUsersRepository _repository;
        private readonly IChatPollRequestsProducer _producer;

        private class PlatformSavedTrigger : ITrigger
        {
            public bool ShouldTrigger(Update update)
            {
                ChatId? chatId = update?.Message?.Chat?.Id;
                
                return chatId != null && ChatPlatforms.ContainsKey(chatId);
            }
        }

        public const string CallbackPath = "platform";

        public ITrigger[] Triggers { get; } = {
            new StartsWithCallbackTrigger(CallbackPath),
            new PlatformSavedTrigger()
        };

        public AddUserCommand(
            ISavedUsersRepository repository,
            IChatPollRequestsProducer producer)
        {
            _repository = repository;
            _producer = producer;
        }

        public Task OperateAsync(ITelegramBotClient client, Update update)
        {
            switch (update.Type)
            {
                case UpdateType.CallbackQuery:
                    return RequestToSendUser(client, update.CallbackQuery);
                
                default:
                    return AddUser(client, update.Message);
            }
        }

        private static Task RequestToSendUser(ITelegramBotClient client, CallbackQuery callbackQuery)
        {
            CallbackQuery query = callbackQuery;

            string platform = query.Data
                .Split("-")
                .Last();
            
            Message message = query.Message;
            ChatId chatId = message.Chat.Id;

            ChatPlatforms.TryAdd(chatId, platform);

            return client.EditMessageTextAsync(
                chatId: chatId,
                messageId: message.MessageId,
                text: $"Enter user from {platform}");
        }
        
        private async Task AddUser(ITelegramBotClient client, Message message)
        {
            ChatId chatId = message.Chat.Id;
            string messageText = message.Text;

            ChatPlatforms.TryRemove(chatId, out string platform);
            
            var user = new User(messageText, messageText, platform);
            TimeSpan interval = TimeSpan.FromMinutes(30);
            
            var userPollRule = new UserPollRule(
                user,
                interval);

            _producer.SendRequest(
                new ChatPollRequest(
                    Request.StartPoll,
                    userPollRule,
                    chatId));

            await _repository.AddOrUpdateAsync(
                user,
                new ChatInfo
                {
                    Chat = chatId,
                    Interval = interval
                });

            await client.SendTextMessageAsync(
                chatId: chatId,
                text: $"Added {messageText} from platform {platform}",
                replyToMessageId: message.MessageId);
        }
    }
}