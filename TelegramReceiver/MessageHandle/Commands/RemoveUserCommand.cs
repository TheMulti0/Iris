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