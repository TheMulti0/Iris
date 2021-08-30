using System.Threading;
using System.Threading.Tasks;

namespace TelegramReceiver
{
    internal class IdCommand : BaseCommand, ICommand
    {
        public IdCommand(Context context) : base(context)
        {
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            await Client.SendTextMessageAsync(
                chatId: ContextChat,
                text: $"{Dictionary.ChatId}: {ContextChat}",
                cancellationToken: token);
            
            return new NoRedirectResult();
        }
    }
}