using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using UserDataLayer;

namespace TelegramReceiver
{
    internal class UsersCommand : ICommand
    {
        private readonly ISavedUsersRepository _repository;
        private readonly IEnumerable<InlineKeyboardButton> _addUserButton;
        private readonly InlineKeyboardMarkup _noUsersMarkup;

        public ITrigger[] Triggers { get; } = { new MessageTextTrigger("/start") };

        public UsersCommand(
            ISavedUsersRepository repository)
        {
            _repository = repository;

            _addUserButton = new[]
            {
                InlineKeyboardButton.WithCallbackData("Add user", SelectPlatformCommand.CallbackPath)
            };
            _noUsersMarkup = new InlineKeyboardMarkup(_addUserButton);
        }

        public Task OperateAsync(ITelegramBotClient client, Update update)
        {
            var msg = update.Message;

            List<SavedUser> currentUsers = _repository
                .Get()
                .Where(user => user.Chats
                           .Any(chat => chat.Chat == (ChatId) msg.Chat.Id))
                .ToList();

            return currentUsers.Any() 
                ? WithUsers(client, msg, currentUsers) 
                : NoUsersFound(client, msg);
        }

        private async Task NoUsersFound(ITelegramBotClient client, Message msg)
        {
            await client.SendTextMessageAsync(
                chatId: msg.Chat.Id,
                text: "No users found",
                replyToMessageId: msg.MessageId,
                replyMarkup: _noUsersMarkup);
        }

        private async Task WithUsers(
            ITelegramBotClient client,
            Message msg,
            List<SavedUser> users)
        {
            IEnumerable<InlineKeyboardButton> userButtons = users
                .Select(ToButton)
                .Concat(_addUserButton);
            
            var inlineKeyboardMarkup = new InlineKeyboardMarkup(userButtons);
            
            await client.SendTextMessageAsync(
                chatId: msg.Chat.Id,
                text: $"{users.Count} users configured",
                replyToMessageId: msg.MessageId,
                replyMarkup: inlineKeyboardMarkup);
        }

        private InlineKeyboardButton ToButton(SavedUser user)
        {
            (string userId, _, string source) = user.User;

            return InlineKeyboardButton.WithCallbackData(
                $"{userId} ({source})");
        }
    }
}