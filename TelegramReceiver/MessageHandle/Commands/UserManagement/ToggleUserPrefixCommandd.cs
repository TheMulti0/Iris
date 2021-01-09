using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Telegram.Bot.Types;
using UserDataLayer;

namespace TelegramReceiver
{
    internal class ToggleUserPrefixCommandd : BaseCommandd, ICommandd
    {
        private readonly ISavedUsersRepository _savedUsersRepository;

        public ToggleUserPrefixCommandd(
            Context context,
            ISavedUsersRepository savedUsersRepository) : base(context)
        {
            _savedUsersRepository = savedUsersRepository;
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            SavedUser savedUser = await _savedUsersRepository.GetAsync(SelectedUser);
            UserChatInfo chat = savedUser.Chats.First(info => info.ChatId == ConnectedChat);

            chat.ShowPrefix = !chat.ShowPrefix;
            
            await _savedUsersRepository.AddOrUpdateAsync(SelectedUser, chat);

            await Client.SendTextMessageAsync(
                chatId: ContextChat,
                text: Dictionary.Enabled, 
                cancellationToken: token);

            return new RedirectResult(Route.User, Context with { Trigger = null });
        }
    }
}