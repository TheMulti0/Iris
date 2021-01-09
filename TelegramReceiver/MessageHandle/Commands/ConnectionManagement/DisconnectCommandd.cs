using System.Threading;
using System.Threading.Tasks;
using TelegramReceiver.Data;

namespace TelegramReceiver
{
    internal class DisconnectCommandd : BaseCommandd, ICommandd
    {
        private readonly IConnectionsRepository _repository;

        public DisconnectCommandd(
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

                return new EmptyResult();
            }

            await _repository.AddOrUpdateAsync(Trigger.GetUser(), ContextChat, Language);

            await Client.SendTextMessageAsync(
                chatId: ContextChat,
                text: $"{Dictionary.DisconnectedFrom} {connectedChatInfo.Title}! ({ConnectedChat})",
                cancellationToken: token);
            
            return new EmptyResult();
        }
    }
}