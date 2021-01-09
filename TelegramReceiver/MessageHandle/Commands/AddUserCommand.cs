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
        private readonly ISavedUsersRepository _savedUsersRepository;
        private readonly IProducer<ChatPollRequest> _producer;
        private readonly TimeSpan _defaultInterval;

        public const string CallbackPath = "platform";

        public ITrigger[] Triggers { get; } = {
            new StartsWithCallbackTrigger(CallbackPath),
        };

        public AddUserCommand(
            TelegramConfig config,
            ISavedUsersRepository savedUsersRepository,
            IProducer<ChatPollRequest> producer)
        {
            _savedUsersRepository = savedUsersRepository;
            _producer = producer;
            
            _defaultInterval = config.DefaultInterval;
        }

        public async Task OperateAsync(Context context)
        {
            CallbackQuery query = context.Trigger.CallbackQuery;

            Platform platform = GetPlatform(query);

            await SendRequestMessage(context, query, platform);

            // Wait for the user to reply with desired user id

            Update newUpdate = await context.IncomingUpdates.FirstAsync(update => update.Type == UpdateType.Message);
            
            await AddUser(context, newUpdate.Message, platform);
        }

        private static Platform GetPlatform(CallbackQuery query)
        {
            return Enum.Parse<Platform>(query.Data
                .Split("-")
                .Last());
        }

        private static Task SendRequestMessage(
            Context context,
            CallbackQuery callbackQuery,
            Platform platform)
        {
            Message message = callbackQuery.Message;

            return context.Client.EditMessageTextAsync(
                chatId: message.Chat.Id,
                messageId: message.MessageId,
                text: $"{context.LanguageDictionary.EnterUserFromPlatform} {context.LanguageDictionary.GetPlatform(platform)}");
        }
        
        private async Task AddUser(Context context, Message message, Platform platform)
        {
            string messageText = message.Text;
            
            var user = new User(messageText, platform);
            TimeSpan interval = _defaultInterval;
            
            var userPollRule = new UserPollRule(user, interval);

            _producer.Send(
                new ChatPollRequest(
                    Request.StartPoll,
                    userPollRule,
                    context.ConnectedChatId));

            await _savedUsersRepository.AddOrUpdateAsync(
                user,
                new UserChatInfo
                {
                    ChatId = context.ConnectedChatId,
                    Interval = interval,
                    DisplayName = user.UserId,
                    Language = context.Language
                });

            await context.Client.SendTextMessageAsync(
                chatId: context.ContextChatId,
                text: $"{context.LanguageDictionary.Added} {user.UserId}",
                replyToMessageId: message.MessageId);
        }
    }
}