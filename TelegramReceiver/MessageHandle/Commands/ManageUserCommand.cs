using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    internal class ManageUserCommand : ICommand
    {
        private readonly ISavedUsersRepository _repository;
        private readonly InlineKeyboardButton[] _backRow;

        public const string CallbackPath = "manageUser";

        public ITrigger[] Triggers { get; } = {
            new StartsWithCallbackTrigger(CallbackPath)
        };

        public ManageUserCommand(
            ISavedUsersRepository repository)
        {
            _repository = repository;

            _backRow = new[]
            {
                InlineKeyboardButton.WithCallbackData("Back", UsersCommand.CallbackPath), 
            };
        }

        public async Task OperateAsync(Context context)
        {
            (ITelegramBotClient client, IObservable<Update> _, Update currentUpdate) = context;
            CallbackQuery query = currentUpdate.CallbackQuery;

            await SendUserInfo(client, query.Message, GetUserBasicInfo(query));
        }

        private static User GetUserBasicInfo(CallbackQuery query)
        {
            string[] items = query.Data.Split("-");
            
            return new User(items[^2], DisplayName: null, items[^1]);
        }

        private Task SendUserInfo(
            ITelegramBotClient client,
            Message message,
            User user)
        {
            (string userId, string displayName, string source) = user;
            
            var text = GetText(userId, displayName, source);

            var inlineKeyboardMarkup = GetMarkup(userId, source);
            
            return client.EditMessageTextAsync(
                chatId: message.Chat.Id,
                messageId: message.MessageId,
                text: text,
                parseMode: ParseMode.Html,
                replyMarkup: inlineKeyboardMarkup);
        }

        private static string GetText(string userId, string displayName, string source)
        {
            var text = new StringBuilder($"Settings for {userId}:");
            
            text.AppendLine($"<b>User id:</b> {userId}");
            text.AppendLine($"<b>Display name:</b> {displayName}");
            text.AppendLine($"<b>Platform:</b> {source}");
            
            return text.ToString();
        }

        private InlineKeyboardMarkup GetMarkup(string userId, string source)
        {
            return new(
                new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(
                            "Set display name",
                            $"{SetUserDisplayNameCommand.CallbackPath}-{userId}-{source}")
                    },
                    _backRow
                });
        }
    }
}