using System;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Telegram.Bot.Types;
using UserDataLayer;
using User = Common.User;

namespace TelegramReceiver
{
    internal class DisableSuffixCommand : ICommand
    {
        private readonly ISavedUsersRepository _savedUsersRepository;

        public const string CallbackPath = "disableSuffix";

        public ITrigger[] Triggers { get; } = {
            new StartsWithCallbackTrigger(CallbackPath)
        };

        public DisableSuffixCommand(
            ISavedUsersRepository savedUsersRepository)
        {
            _savedUsersRepository = savedUsersRepository;
        }

        public async Task OperateAsync(Context context)
        {
            CallbackQuery query = context.Update.CallbackQuery;

            User user = GetUserBasicInfo(query);

            SavedUser savedUser = await _savedUsersRepository.GetAsync(user);
            UserChatInfo chat = savedUser.Chats.First(info => info.ChatId == context.ConnectedChatId);

            chat.ShowSuffix = false;
            
            await _savedUsersRepository.AddOrUpdateAsync(user, chat);

            await context.Client.SendTextMessageAsync(
                chatId: context.ContextChatId,
                text: context.LanguageDictionary.Disabled);
        }

        private static User GetUserBasicInfo(CallbackQuery query)
        {
            string[] items = query.Data.Split("-");
            
            return new User(items[^2], Enum.Parse<Platform>(items[^1]));
        }
    }
}