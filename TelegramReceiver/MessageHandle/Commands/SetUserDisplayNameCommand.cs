using System;
using System.Linq;
using System.Reactive.Linq;
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
    internal class SetUserDisplayNameCommand : ICommand
    {
        private readonly IConnectionsRepository _connectionsRepository;
        private readonly ISavedUsersRepository _savedUsersRepository;

        public const string CallbackPath = "setDisplayName";

        public ITrigger[] Triggers { get; } = {
            new StartsWithCallbackTrigger(CallbackPath)
        };

        public SetUserDisplayNameCommand(
            IConnectionsRepository connectionsRepository,
            ISavedUsersRepository savedUsersRepository)
        {
            _connectionsRepository = connectionsRepository;
            _savedUsersRepository = savedUsersRepository;
        }

        public async Task OperateAsync(Context context)
        {
            (ITelegramBotClient client, IObservable<Update> incoming, Update currentUpdate) = context;
            CallbackQuery query = currentUpdate.CallbackQuery;

            User user = GetUserBasicInfo(query);

            InlineKeyboardMarkup inlineKeyboardMarkup = CreateMarkup(user);

            await SendRequestMessage(client, query.Message, inlineKeyboardMarkup);

            // Wait for the user to reply with desired display name
            
            var update = await incoming.FirstAsync(u => u.Type == UpdateType.Message);

            await SetDisplayName(user, update, client, inlineKeyboardMarkup);
        }

        private async Task SetDisplayName(
            User user,
            Update update,
            ITelegramBotClient client,
            IReplyMarkup markup)
        {
            ChatId contextChat = update.GetChatId();
            ChatId connectedChat = await _connectionsRepository.GetAsync(update.GetUser()) ?? contextChat;

            SavedUser savedUser = await _savedUsersRepository.GetAsync(user);
            UserChatInfo chat = savedUser.Chats.First(info => info.ChatId == connectedChat);

            string newDisplayName = update.Message.Text;
            chat.DisplayName = newDisplayName;
            
            await _savedUsersRepository.AddOrUpdateAsync(user, chat);

            await client.SendTextMessageAsync(
                chatId: contextChat,
                text: $"Updated display name to {newDisplayName}",
                replyMarkup: markup);
        }

        private static InlineKeyboardMarkup CreateMarkup(User user)
        {
            (string userId, Platform platform) = user;
            
            return new InlineKeyboardMarkup(
                InlineKeyboardButton.WithCallbackData(
                    "Back",
                    $"{ManageUserCommand.CallbackPath}-{userId}-{Enum.GetName(platform)}"));
        }

        private static User GetUserBasicInfo(CallbackQuery query)
        {
            string[] items = query.Data.Split("-");
            
            return new User(items[^2], Enum.Parse<Platform>(items[^1]));
        }

        private static Task SendRequestMessage(
            ITelegramBotClient client,
            Message message,
            InlineKeyboardMarkup markup)
        {
            return client.EditMessageTextAsync(
                chatId: message.Chat.Id,
                messageId: message.MessageId,
                text: "Enter new display name",
                replyMarkup: markup);
        }
    }
}