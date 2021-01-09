using System.Threading;
using System.Threading.Tasks;
using Common;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramReceiver.Data;
using Update = Telegram.Bot.Types.Update;

namespace TelegramReceiver
{
    internal class DisconnectCommandd : ICommandd
    {
        private readonly ITelegramBotClient _client;
        private readonly Update _update;
        private readonly ChatId _contextChat;
        private readonly ChatId _connectedChat;
        private readonly Language _language;
        private readonly LanguageDictionary _dictionary;
        private readonly Chat _connectedChatInfo;
        
        private readonly IConnectionsRepository _repository;

        public DisconnectCommandd(
            Context context,
            IConnectionsRepository repository)
        {
            (_client, _, _update, _contextChat, _connectedChat, _language, _dictionary) = context;

            _connectedChatInfo = context.ConnectedChat;
            
            _repository = repository;
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            var connectedChatInfo = _connectedChatInfo ?? await _client.GetChatAsync(_connectedChat, token);
            
            if (Equals(_contextChat, _connectedChat))
            {
                await _client.SendTextMessageAsync(
                    chatId: _connectedChat,
                    text: $"{_dictionary.DisconnectedFrom} {connectedChatInfo.Title}! ({_connectedChat})",
                    cancellationToken: token);

                return new EmptyResult();
            }

            await _repository.AddOrUpdateAsync(_update.GetUser(), _contextChat, _language);

            await _client.SendTextMessageAsync(
                chatId: _contextChat,
                text: $"{_dictionary.DisconnectedFrom} {connectedChatInfo.Title}! ({_connectedChat})",
                cancellationToken: token);
            
            return new EmptyResult();
        }
    }
}