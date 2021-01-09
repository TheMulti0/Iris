using System.Threading;
using System.Threading.Tasks;
using Common;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Message = Telegram.Bot.Types.Message;

namespace TelegramReceiver
{
    internal class ConnectionCommandd : BaseCommandd, ICommandd
    {
        public ConnectionCommandd(Context context): base(context)
        {
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            Chat connectedChatInfo = Context.ConnectedChat ?? await Client.GetChatAsync(ConnectedChat, token);

            if (connectedChatInfo.Type == ChatType.Private)
            {
                await Client.SendTextMessageAsync(
                    chatId: ContextChat,
                    text: Dictionary.NotConnected,
                    cancellationToken: token);
                
                return new EmptyResult();
            }
            
            await Client.SendTextMessageAsync(
                chatId: ContextChat,
                text: $"{Dictionary.ConnectedToChat} {connectedChatInfo.Title}! ({ConnectedChat})",
                cancellationToken: token);
            
            return new EmptyResult();
        }
    }
}