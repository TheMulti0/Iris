using System;
using System.Threading.Tasks;
using Common;
using Extensions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using UserDataLayer;
using Message = Telegram.Bot.Types.Message;
using User = Common.User;

namespace TelegramReceiver
{
    internal class RemoveUserCommand : ICommand
    {
        private readonly ISavedUsersRepository _savedUsersRepository;
        private readonly IProducer<ChatPollRequest> _producer;

        public const string CallbackPath = "remove";

        public ITrigger[] Triggers { get; } = {
            new StartsWithCallbackTrigger(CallbackPath)
        };

        public RemoveUserCommand(
            ISavedUsersRepository savedUsersRepository,
            IProducer<ChatPollRequest> producer)
        {
            _savedUsersRepository = savedUsersRepository;
            _producer = producer;
        }

        public async Task OperateAsync(Context context)
        {
            CallbackQuery query = context.Update.CallbackQuery;

            User user = GetUserBasicInfo(query);

            InlineKeyboardMarkup inlineKeyboardMarkup = CreateMarkup(context, user);

            await Remove(context, user, query.Message, inlineKeyboardMarkup);
        }

        private async Task Remove(
            Context context,
            User user,
            Message message,
            InlineKeyboardMarkup markup)
        {
            var userPollRule = new UserPollRule(user, null);
            
            _producer.Send(
                new ChatPollRequest(
                    Request.StopPoll,
                    userPollRule,
                    context.ConnectedChatId));
            
            await _savedUsersRepository.RemoveAsync(user, context.ConnectedChatId);

            await context.Client.EditMessageTextAsync(
                messageId: message.MessageId,
                chatId: context.ContextChatId,
                text: $"{context.LanguageDictionary.Removed} ({user.UserId})",
                replyMarkup: markup);
        }

        private static InlineKeyboardMarkup CreateMarkup(Context context, User user)
        {
            (string userId, Platform platform) = user;
            
            return new InlineKeyboardMarkup(
                InlineKeyboardButton.WithCallbackData(
                    context.LanguageDictionary.Back,
                    $"{ManageUserCommand.CallbackPath}-{userId}-{Enum.GetName(platform)}"));
        }

        private static User GetUserBasicInfo(CallbackQuery query)
        {
            string[] items = query.Data.Split("-");
            
            return new User(
                items[^2],
                Enum.Parse<Platform>(items[^1]));
        }
    }
}