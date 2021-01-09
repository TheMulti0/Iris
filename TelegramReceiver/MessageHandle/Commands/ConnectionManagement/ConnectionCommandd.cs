using System.Threading;
using System.Threading.Tasks;
using Common;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Message = Telegram.Bot.Types.Message;

namespace TelegramReceiver
{
    internal class ConnectionCommandd : ICommandd
    {
        private readonly ITelegramBotClient _client;
        private readonly ChatId _contextChat;
        private readonly ChatId _connectedChat;
        private readonly LanguageDictionary _dictionary;
        private readonly Chat _connectedChatInfo;

        public ConnectionCommandd(
            Context context)
        {
            (_client, _, _, _contextChat, _connectedChat, _, _dictionary) = context;

            _connectedChatInfo = context.ConnectedChat;
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            Chat connectedChatInfo = _connectedChatInfo ?? await _client.GetChatAsync(_connectedChat, token);

            if (connectedChatInfo.Type == ChatType.Private)
            {
                await _client.SendTextMessageAsync(
                    chatId: _contextChat,
                    text: _dictionary.NotConnected,
                    cancellationToken: token);
                
                return new EmptyResult();
            }
            
            await _client.SendTextMessageAsync(
                chatId: _contextChat,
                text: $"{_dictionary.ConnectedToChat} {connectedChatInfo.Title}! ({_connectedChat})",
                cancellationToken: token);
            
            return new EmptyResult();
        }
    }
}