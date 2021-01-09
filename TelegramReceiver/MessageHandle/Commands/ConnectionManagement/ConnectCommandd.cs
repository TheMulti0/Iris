using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using TelegramReceiver.Data;
using Message = Telegram.Bot.Types.Message;
using Update = Telegram.Bot.Types.Update;

namespace TelegramReceiver
{
    internal class ConnectCommandd : ICommandd
    {
        private readonly Context _context;
        private readonly ITelegramBotClient _client;
        private readonly Update _update;
        private readonly ChatId _contextChat;
        private readonly Language _language;
        private readonly LanguageDictionary _dictionary;

        private readonly IConnectionsRepository _repository;

        public ConnectCommandd(
            Context context,
            IConnectionsRepository repository)
        {
            _context = context;
            
            (_client, _, _update, _contextChat, _, _language, _dictionary) = context;
            _repository = repository;
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            Message message = _update.Message;
            string[] arguments = message.Text.Split(' ');

            if (arguments.Length <= 1)
            {
                await _client.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: _dictionary.NoChatId,
                    cancellationToken: token);
                return new EmptyResult();
            }

            var chatId = (ChatId) arguments[1];
            Chat chat;
            try
            {
                chat = await _client.GetChatAsync(chatId, token);
            }
            catch (ChatNotFoundException)
            {
                await _client.SendTextMessageAsync(
                    chatId: _contextChat,
                    text: _dictionary.NoChat,
                    cancellationToken: token);
                return new EmptyResult();
            }
            
            ChatMember[] administrators = await _client.GetChatAdministratorsAsync(chatId, token);

            if (administrators.All(member => member.User.Id != message.From.Id))
            {
                await _client.SendTextMessageAsync(
                    chatId: _contextChat,
                    text: _dictionary.NotAdmin,
                    cancellationToken: token);
                return new EmptyResult();
            }

            await _repository.AddOrUpdateAsync(message.From, chatId, _language);

            string chatTitle = chat.Title != null 
                ? $" {chat.Title}" 
                : string.Empty;
            
            await _client.SendTextMessageAsync(
                chatId: _contextChat,
                text: $"{_dictionary.ConnectedToChat}{chatTitle}! ({chatId})",
                cancellationToken: token);

            return new RedirectResult(Route.Connection, _context with { ConnectedChatId = chat });
        }
    }
}