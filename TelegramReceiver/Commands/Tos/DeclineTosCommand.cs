using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramReceiver
{
    internal class DeclineTosCommand : BaseCommand, ICommand
    {
        public DeclineTosCommand(
            Context context) : base(context)
        {
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            await Client.EditMessageTextAsync(
                chatId: ContextChat,
                messageId: Trigger.GetMessageId(),
                text: Dictionary.ThanksForCheckingOut,
                cancellationToken: token);
            
            return new NoRedirectResult();
        }
    }
}