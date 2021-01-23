using System.Threading;
using System.Threading.Tasks;

namespace TelegramReceiver
{
    internal class StartCommand : BaseCommand, ICommand
    {
        public StartCommand(Context context) : base(context)
        {
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            await Client.SendTextMessageAsync(
                chatId: ContextChat,
                text: Dictionary.Start,
                cancellationToken: token);
            
            return new NoRedirectResult();
        }
    }
}