using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using MoreLinq.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramReceiver.Data;
using UserDataLayer;
using Update = Telegram.Bot.Types.Update;

namespace TelegramReceiver
{
    internal class UsersCommand : ICommand
    {
        private readonly IConnectionsRepository _connectionsRepository;
        private readonly ISavedUsersRepository _savedUsersRepository;
        private readonly IEnumerable<InlineKeyboardButton> _addUserButton;
        private readonly InlineKeyboardMarkup _noUsersMarkup;

        public const string CallbackPath = "home";
        
        public ITrigger[] Triggers { get; } = {
            new MessageTextTrigger("/start"),
            new CallbackTrigger(CallbackPath)
        };

        public UsersCommand(
            IConnectionsRepository connectionsRepository,
            ISavedUsersRepository savedUsersRepository)
        {
            _connectionsRepository = connectionsRepository;
            _savedUsersRepository = savedUsersRepository;

            _addUserButton = new[]
            {
                InlineKeyboardButton.WithCallbackData("Add user", SelectPlatformCommand.CallbackPath)
            };
            _noUsersMarkup = new InlineKeyboardMarkup(_addUserButton);
        }

        public async Task OperateAsync(Context context)
        {
            (ITelegramBotClient client, _, Update update) = context;
            ChatId contextChat = update.GetChatId();
            ChatId connectedChat = await _connectionsRepository.GetAsync(update.GetUser()) ?? contextChat;

            List<SavedUser> currentUsers = _savedUsersRepository
                .GetAll()
                .Where(
                    user => user.Chats
                        .Any(chat => chat.ChatId == connectedChat))
                .ToList();

            (InlineKeyboardMarkup markup, string text) = Get(currentUsers);

            if (update.Type == UpdateType.CallbackQuery)
            {
                await client.EditMessageTextAsync(
                    chatId: contextChat,
                    messageId: update.CallbackQuery.Message.MessageId,
                    text: text,
                    replyMarkup: markup);
                return;
            }
            
            await client.SendTextMessageAsync(
                chatId: contextChat,
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
            (string userId, Platform platform) = user.User;

            return InlineKeyboardButton.WithCallbackData(
                $"{user.User}",
                $"{ManageUserCommand.CallbackPath}-{userId}-{Enum.GetName(platform)}");
        }
    }
}