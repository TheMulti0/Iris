using System.Threading;
using System.Threading.Tasks;

namespace TelegramReceiver
{
    internal class DisconnectCommand : BaseCommand, ICommand
    {
        private readonly IConnectionsRepository _repository;

        public DisconnectCommand(
            Context context,
            IConnectionsRepository repository) : base(context)
        {
            _repository = repository;
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            var connectedChatInfo = Context.ConnectedChat ?? await Client.GetChatAsync(ConnectedChat, token);
            
            if (Equals(ConnectedChat, ContextChat))
            {
                await Client.SendTextMessageAsync(
                    chatId: ContextChat,
                    text: $"{Dictionary.DisconnectedFrom} {connectedChatInfo.Title}! ({ConnectedChat})",
                    cancellationToken: token);

                return new NoRedirectResult();
            }

            Connection.Chat = ContextChat;
            await _repository.AddOrUpdateAsync(Trigger.GetUser(), Connection);

            await Client.SendTextMessageAsync(
                chatId: ContextChat,
                text: $"{Dictionary.DisconnectedFrom} {connectedChatInfo.Title}! ({ConnectedChat})",
                cancellationToken: token);
            
            return new NoRedirectResult();
        }
    }
}