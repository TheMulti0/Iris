using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramReceiver.Data;
using UserDataLayer;
using User = Common.User;

namespace TelegramReceiver
{
    internal class RemoveUserCommand : ICommand
    {
        private readonly IConnectionsRepository _connectionsRepository;
        private readonly ISavedUsersRepository _savedUsersRepository;

        public const string CallbackPath = "remove";

        public ITrigger[] Triggers { get; } = {
            new StartsWithCallbackTrigger(CallbackPath)
        };

        public RemoveUserCommand(
            IConnectionsRepository connectionsRepository,
            ISavedUsersRepository savedUsersRepository)
        {
            _connectionsRepository = connectionsRepository;
            _savedUsersRepository = savedUsersRepository;
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
            ChatId contextChat = message.Chat.Id;
            ChatId connectedChat = await _connectionsRepository.GetAsync(message.From) ?? contextChat;

            await _savedUsersRepository.RemoveAsync(user, connectedChat);

            await client.EditMessageTextAsync(
                messageId: message.MessageId,
                chatId: contextChat,
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
    }
}