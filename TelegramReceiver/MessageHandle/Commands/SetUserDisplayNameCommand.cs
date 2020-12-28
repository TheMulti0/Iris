using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Common;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UserDataLayer;
using Message = Telegram.Bot.Types.Message;
using Update = Telegram.Bot.Types.Update;
using User = Common.User;

namespace TelegramReceiver
{
    internal class SetUserDisplayNameCommand : ICommand
    {
        private readonly ISavedUsersRepository _savedUsersRepository;

        public const string CallbackPath = "setDisplayName";

        public ITrigger[] Triggers { get; } = {
            new StartsWithCallbackTrigger(CallbackPath)
        };

        public SetUserDisplayNameCommand(
            ISavedUsersRepository savedUsersRepository)
        {
            _savedUsersRepository = savedUsersRepository;
        }

        public async Task OperateAsync(Context context)
        {
            CallbackQuery query = context.Update.CallbackQuery;

            User user = GetUserBasicInfo(query);

            InlineKeyboardMarkup inlineKeyboardMarkup = CreateMarkup(context, user);

            await SendRequestMessage(context, query.Message, inlineKeyboardMarkup);

            // Wait for the user to reply with desired display name
            
            var update = await context.IncomingUpdates.FirstAsync(u => u.Type == UpdateType.Message);

            await SetDisplayName(context, user, update, inlineKeyboardMarkup);
        }

        private async Task SetDisplayName(
            Context context,
            User user,
            Update update,
            IReplyMarkup markup)
        {
            SavedUser savedUser = await _savedUsersRepository.GetAsync(user);
            UserChatInfo chat = savedUser.Chats.First(info => info.ChatId == context.ConnectedChatId);

            string newDisplayName = update.Message.Text;
            chat.DisplayName = newDisplayName;
            
            await _savedUsersRepository.AddOrUpdateAsync(user, chat);

            await context.Client.SendTextMessageAsync(
                chatId: context.ContextChatId,
                text: $"{context.LanguageDictionary.UpdatedDisplayName} {newDisplayName}",
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
            
            return new User(items[^2], Enum.Parse<Platform>(items[^1]));
        }

        private static Task SendRequestMessage(
            Context context,
            Message message,
            InlineKeyboardMarkup markup)
        {
            return context.Client.EditMessageTextAsync(
                chatId: message.Chat.Id,
                messageId: message.MessageId,
                text: context.LanguageDictionary.EnterNewDisplayName,
                replyMarkup: markup);
        }
    }
}