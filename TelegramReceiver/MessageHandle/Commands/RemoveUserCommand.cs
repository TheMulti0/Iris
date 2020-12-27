using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UserDataLayer;
using User = Common.User;

namespace TelegramReceiver
{
    internal class RemoveUserCommand : ICommand
    {
        private readonly ISavedUsersRepository _repository;

        public const string CallbackPath = "remove";

        public ITrigger[] Triggers { get; } = {
            new StartsWithCallbackTrigger(CallbackPath)
        };

        public RemoveUserCommand(
            ISavedUsersRepository repository)
        {
            _repository = repository;
        }

        public async Task OperateAsync(Context context)
        {
            (ITelegramBotClient client, _, Update currentUpdate) = context;
            CallbackQuery query = currentUpdate.CallbackQuery;

            User user = GetUserBasicInfo(query);

            InlineKeyboardMarkup inlineKeyboardMarkup = CreateMarkup(user);

            await Remove(user, query.Message, client, inlineKeyboardMarkup);
        }

        private async Task Remove(
            User user,
            Message message,
            ITelegramBotClient client,
            InlineKeyboardMarkup markup)
        {
            var chatId = (ChatId) message.Chat.Id;

            await _repository.RemoveAsync(user, chatId);

            await client.EditMessageTextAsync(
                messageId: message.MessageId,
                chatId: chatId,
                text: $"Removed {user.UserId}",
                replyMarkup: markup);
        }

        private static InlineKeyboardMarkup CreateMarkup(User user)
        {
            (string userId, string source) = user;
            
            return new InlineKeyboardMarkup(
                InlineKeyboardButton.WithCallbackData(
                    "Back",
                    $"{ManageUserCommand.CallbackPath}-{userId}-{source}"));
        }

        private static User GetUserBasicInfo(CallbackQuery query)
        {
            string[] items = query.Data.Split("-");
            
            return new User(items[^2], DisplayName: null, items[^1]);
        }

        private static Task SendRequestMessage(
            ITelegramBotClient client,
            Message message,
            InlineKeyboardMarkup markup)
        {
            return client.EditMessageTextAsync(
                chatId: message.Chat.Id,
                messageId: message.MessageId,
                text: " ",
                replyMarkup: markup);
        }
    }
}