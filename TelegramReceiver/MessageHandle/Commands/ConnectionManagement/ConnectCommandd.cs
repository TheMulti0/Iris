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
    internal class ConnectCommandd : BaseCommandd, ICommandd
    {
        private readonly IConnectionsRepository _repository;

        public ConnectCommandd(
            Context context,
            IConnectionsRepository repository): base(context)
        {
            _repository = repository;
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            Message message = Trigger.Message;
            string[] arguments = message.Text.Split(' ');

            if (arguments.Length <= 1)
            {
                await Client.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: Dictionary.NoChatId,
                    cancellationToken: token);
                return new EmptyResult();
            }

            var chatId = (ChatId) arguments[1];
            Chat chat;
            try
            {
                chat = await Client.GetChatAsync(chatId, token);
            }
            catch (ChatNotFoundException)
            {
                await Client.SendTextMessageAsync(
                    chatId: ContextChat,
                    text: Dictionary.NoChat,
                    cancellationToken: token);
                return new EmptyResult();
            }
            
            ChatMember[] administrators = await Client.GetChatAdministratorsAsync(chatId, token);

            if (administrators.All(member => member.User.Id != message.From.Id))
            {
                await Client.SendTextMessageAsync(
                    chatId: ContextChat,
                    text: Dictionary.NotAdmin,
                    cancellationToken: token);
                return new EmptyResult();
            }

            await _repository.AddOrUpdateAsync(message.From, chatId, Language);

            string chatTitle = chat.Title != null 
                ? $" {chat.Title}" 
                : string.Empty;
            
            await Client.SendTextMessageAsync(
                chatId: ContextChat,
                text: $"{Dictionary.ConnectedToChat}{chatTitle}! ({chatId})",
                cancellationToken: token);

            return new RedirectResult(Route.Connection, Context with { ConnectedChatId = chat });
        }
    }
}