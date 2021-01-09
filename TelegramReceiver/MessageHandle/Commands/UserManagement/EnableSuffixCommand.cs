using System;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Telegram.Bot.Types;
using UserDataLayer;
using User = Common.User;

namespace TelegramReceiver
{
    internal class EnableSuffixCommand : ICommand
    {
        private readonly ISavedUsersRepository _savedUsersRepository;

        public const string CallbackPath = "enableSuffix";

        public ITrigger[] Triggers { get; } = {
            new StartsWithCallbackTrigger(CallbackPath)
        };

        public EnableSuffixCommand(
            ISavedUsersRepository savedUsersRepository)
        {
            _savedUsersRepository = savedUsersRepository;
        }

        public async Task OperateAsync(Context context)
        {
            CallbackQuery query = context.Trigger.CallbackQuery;

            User user = GetUserBasicInfo(query);

            SavedUser savedUser = await _savedUsersRepository.GetAsync(user);
            UserChatInfo chat = savedUser.Chats.First(info => info.ChatId == context.ConnectedChatId);

            chat.ShowSuffix = true;
            
            await _savedUsersRepository.AddOrUpdateAsync(user, chat);

            await context.Client.SendTextMessageAsync(
                chatId: context.ContextChatId,
                text: context.LanguageDictionary.Enabled);
        }

        private static User GetUserBasicInfo(CallbackQuery query)
        {
            string[] items = query.Data.Split("-");
            
            return new User(items[^2], Enum.Parse<Platform>(items[^1]));
        }
    }
}