using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Common;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UserDataLayer;
using User = Common.User;

namespace TelegramReceiver
{
    internal class EnablePrefixCommand : ICommand
    {
        private readonly ISavedUsersRepository _savedUsersRepository;

        public const string CallbackPath = "enablePrefix";

        public ITrigger[] Triggers { get; } = {
            new StartsWithCallbackTrigger(CallbackPath)
        };

        public EnablePrefixCommand(
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

            chat.ShowPrefix = true;
            
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