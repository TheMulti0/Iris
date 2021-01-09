using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Telegram.Bot.Types;
using UserDataLayer;

namespace TelegramReceiver
{
    internal class ToggleUserPrefixCommand : BaseCommand, ICommand
    {
        private readonly ISavedUsersRepository _savedUsersRepository;

        public ToggleUserPrefixCommand(
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

            return new RedirectResult(Route.User);
        }
    }
}