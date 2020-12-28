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
        private readonly ISavedUsersRepository _savedUsersRepository;
        private readonly Languages _languages;

        public const string CallbackPath = "manageUser";

        public ITrigger[] Triggers { get; } = {
            new StartsWithCallbackTrigger(CallbackPath)
        };

        public ManageUserCommand(
            ISavedUsersRepository savedUsersRepository,
            Languages languages)
        {
            _savedUsersRepository = savedUsersRepository;
            _languages = languages;
        }

        public async Task OperateAsync(Context context)
        {
            CallbackQuery query = context.Update.CallbackQuery;

            await SendUserInfo(context, query.Message, GetUserBasicInfo(query));
        }

        private static User GetUserBasicInfo(CallbackQuery query)
        {
            string[] items = query.Data.Split("-");
            
            return new User(items[^2], Enum.Parse<Platform>(items[^1]));
        }

        private async Task SendUserInfo(
            Context context,
            Message message,
            User user)
        {
            var savedUser = await _savedUsersRepository.GetAsync(user);

            UserChatInfo chatInfo = savedUser.Chats.First(info => info.ChatId == context.ConnectedChatId);
            
            var text = GetText(
                context,
                user,
                chatInfo);

            var inlineKeyboardMarkup = GetMarkup(context, user);
            
            await context.Client.EditMessageTextAsync(
                chatId: context.ContextChatId,
                messageId: message.MessageId,
                text: text,
                parseMode: ParseMode.Html,
                replyMarkup: inlineKeyboardMarkup);
        }

        private string GetText(Context context, User user, UserChatInfo info)
        {
            var text = new StringBuilder($"{context.LanguageDictionary.SettingsFor} {user}:");
            text.AppendLine("\n");
            text.AppendLine($"<b>{context.LanguageDictionary.UserId}:</b> {user.UserId}");
            text.AppendLine($"<b>{context.LanguageDictionary.Platform}:</b> {user.Platform}");
            text.AppendLine($"<b>{context.LanguageDictionary.DisplayName}:</b> {info.DisplayName}");
            text.AppendLine($"<b>{context.LanguageDictionary.MaxDelay}:</b> {info.Interval * 2}");
            text.AppendLine($"<b>{context.LanguageDictionary.Language}:</b> {_languages.Dictionary[info.Language].LanguageString}");
            
            return text.ToString();
        }

        private static InlineKeyboardMarkup GetMarkup(Context context, User user)
        {
            return new(
                new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(
                            context.LanguageDictionary.SetDisplayName,
                            $"{SetUserDisplayNameCommand.CallbackPath}-{user.UserId}-{user.Platform}")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(
                            context.LanguageDictionary.Language,
                            $"{SetUserLanguageCommand.CallbackPath}-{user.UserId}-{user.Platform}")
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(
                            context.LanguageDictionary.Remove,
                            $"{RemoveUserCommand.CallbackPath}-{user.UserId}-{user.Platform}"),                        
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(
                            context.LanguageDictionary.Back,
                            UsersCommand.CallbackPath), 
                    }
                });
        }
    }
}