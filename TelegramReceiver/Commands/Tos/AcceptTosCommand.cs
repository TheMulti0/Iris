using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;

namespace TelegramReceiver
{
    internal class AcceptTosCommand : BaseCommand, ICommand
    {
        private readonly IConnectionsRepository _connectionsRepository;

        public AcceptTosCommand(
            Context context,
            IConnectionsRepository connectionsRepository) : base(context)
        {
            _connectionsRepository = connectionsRepository;
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            Connection.HasAgreedToTos = true;
            await _connectionsRepository.AddOrUpdateAsync(
                Trigger.GetUser(),
                Connection);
            
            await Client.EditMessageTextAsync(
                chatId: ContextChat,
                messageId: Trigger.GetMessageId(),
                text: Dictionary.ThanksNowYouCanUse,
                cancellationToken: token);

            return new NoRedirectResult();
        }
    }
}