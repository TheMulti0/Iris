using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using UserDataLayer;

namespace TelegramReceiver
{
    internal class ToggleUserSendScreenshotOnlyCommand : BaseCommand, ICommand
    {
        private readonly ISavedUsersRepository _savedUsersRepository;

        public ToggleUserSendScreenshotOnlyCommand(
            Context context,
            ISavedUsersRepository savedUsersRepository) : base(context)
        {
            _savedUsersRepository = savedUsersRepository;
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            SavedUser savedUser = await _savedUsersRepository.GetAsync(SelectedUser);
            UserChatSubscription chat = savedUser.Chats.First(info => info.ChatId == ConnectedChat);

            chat.SendScreenshotOnly = !chat.SendScreenshotOnly;
            
            await _savedUsersRepository.AddOrUpdateAsync(SelectedUser, chat);

            return new RedirectResult(Route.User);
        }
    }
}