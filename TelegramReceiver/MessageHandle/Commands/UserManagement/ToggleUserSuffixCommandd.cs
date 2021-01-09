using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using UserDataLayer;

namespace TelegramReceiver
{
    internal class ToggleUserSuffixCommandd : BaseCommandd, ICommandd
    {
        private readonly ISavedUsersRepository _savedUsersRepository;

        public ToggleUserSuffixCommandd(
            Context context,
            ISavedUsersRepository savedUsersRepository) : base(context)
        {
            _savedUsersRepository = savedUsersRepository;
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            SavedUser savedUser = await _savedUsersRepository.GetAsync(SelectedUser);
            UserChatInfo chat = savedUser.Chats.First(info => info.ChatId == ConnectedChat);

            chat.ShowSuffix = !chat.ShowSuffix;
            
            await _savedUsersRepository.AddOrUpdateAsync(SelectedUser, chat);

            await Client.SendTextMessageAsync(
                chatId: ContextChat,
                text: Dictionary.Enabled, 
                cancellationToken: token);

            return new RedirectResult(Route.User, Context with { Trigger = null });
        }
    }
}