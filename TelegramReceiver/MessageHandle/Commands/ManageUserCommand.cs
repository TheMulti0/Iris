using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Common;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramReceiver.Data;
using UserDataLayer;
using Message = Telegram.Bot.Types.Message;
using Update = Telegram.Bot.Types.Update;
using User = Common.User;

namespace TelegramReceiver
{
    internal class ManageUserCommand : ICommand
    {
        private readonly IConnectionsRepository _connectionsRepository;
        private readonly ISavedUsersRepository _savedUsersRepository;
        private readonly InlineKeyboardButton[] _backRow;

        public const string CallbackPath = "manageUser";

        public ITrigger[] Triggers { get; } = {
            new StartsWithCallbackTrigger(CallbackPath)
        };

        public ManageUserCommand(
            IConnectionsRepository connectionsRepository,
            ISavedUsersRepository savedUsersRepository)
        {
            _connectionsRepository = connectionsRepository;
            _savedUsersRepository = savedUsersRepository;

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
            
            return new User(items[^2], Enum.Parse<Platform>(items[^1]));
        }

        private async Task SendUserInfo(
            ITelegramBotClient client,
            Message message,
            User user)
        {
            ChatId contextChat = message.Chat.Id;
            ChatId connectedChat = await _connectionsRepository.GetAsync(message.From) ?? contextChat;
            
            var savedUser = await _savedUsersRepository.GetAsync(user);
            
            var text = GetText(user, savedUser.Chats.First(info => info.ChatId == connectedChat));

            var inlineKeyboardMarkup = GetMarkup(user);
            
            await client.EditMessageTextAsync(
                chatId: contextChat,
                messageId: message.MessageId,
                text: text,
                parseMode: ParseMode.Html,
                replyMarkup: inlineKeyboardMarkup);
        }

        private static string GetText(User user, UserChatInfo info)
        {
            var text = new StringBuilder($"Settings for {user}:");
            
            text.AppendLine($"<b>User id:</b> {user.UserId}");
            text.AppendLine($"<b>Platform:</b> {user.Platform}");
            text.AppendLine($"<b>Display name:</b> {info.DisplayName}");
            text.AppendLine($"<b>Max delay:</b> up to {info.Interval * 2}");
            
            return text.ToString();
        }

        private InlineKeyboardMarkup GetMarkup(User user)
        {
            return new(
                new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(
                            "Set display name",
                            $"{SetUserDisplayNameCommand.CallbackPath}-{user.UserId}-{user.Platform}")
                    },
                    _backRow
                });
        }
    }
}