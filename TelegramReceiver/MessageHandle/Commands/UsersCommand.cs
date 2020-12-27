using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoreLinq.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UserDataLayer;

namespace TelegramReceiver
{
    internal class UsersCommand : ICommand
    {
        private readonly ISavedUsersRepository _repository;
        private readonly IEnumerable<InlineKeyboardButton> _addUserButton;
        private readonly InlineKeyboardMarkup _noUsersMarkup;

        public const string CallbackPath = "home";
        
        public ITrigger[] Triggers { get; } = {
            new MessageTextTrigger("/start"),
            new CallbackTrigger(CallbackPath)
        };

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

        public Task OperateAsync(Context context)
        {
            (ITelegramBotClient client, _, Update update) = context;
            var chatId = update.GetChatId();

            List<SavedUser> currentUsers = _repository
                .GetAll()
                .Where(
                    user => user.Chats
                        .Any(chat => chat.Chat == chatId))
                .ToList();

            (InlineKeyboardMarkup markup, string text) = Get(currentUsers);

            if (update.Type == UpdateType.CallbackQuery)
            {
                return client.EditMessageTextAsync(
                    chatId: chatId,
                    messageId: update.CallbackQuery.Message.MessageId,
                    text: text,
                    replyMarkup: markup);
            }
            
            return client.SendTextMessageAsync(
                chatId: chatId,
                text: text,
                replyMarkup: markup);
    }

        private (InlineKeyboardMarkup, string) Get(IReadOnlyCollection<SavedUser> currentUsers)
        {
            return currentUsers.Any() 
                ? (GetUsersMarkup(currentUsers), $"{currentUsers.Count} users found") 
                : (_noUsersMarkup, "No users found");
        }

        private InlineKeyboardMarkup GetUsersMarkup(IEnumerable<SavedUser> users)
        {
            IEnumerable<IEnumerable<InlineKeyboardButton>> userButtons = users
                .Select(ToButton)
                .Batch(2)
                .Concat(
                    new[]
                    {
                        _addUserButton
                    });
            
            return new InlineKeyboardMarkup(userButtons);
        }

        private static InlineKeyboardButton ToButton(SavedUser user)
        {
            (string userId, string source) = user.User;

            return InlineKeyboardButton.WithCallbackData(
                $"{userId} ({source})",
                $"{ManageUserCommand.CallbackPath}-{userId}-{source}");
        }
    }
}