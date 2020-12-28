using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramReceiver.Data;
using Message = Telegram.Bot.Types.Message;

namespace TelegramReceiver
{
    internal class ConnectionCommand : ICommand
    {
        private readonly IConnectionsRepository _repository;

        public ITrigger[] Triggers { get; } = {
            new MessageStartsWithTextTrigger("/connection")
        };

        public ConnectionCommand(
            IConnectionsRepository repository)
        {
            _repository = repository;
        }

        public async Task OperateAsync(Context context)
        {
            Message message = context.Update.Message;
            var connection = await _repository.GetAsync(message.From);
            Chat connectedChat = await context.Client.GetChatAsync(connection.Chat);

            if (connectedChat.Type == ChatType.Private)
            {
                await context.Client.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: context.LanguageDictionary.NotConnected);
                return;
            }
            
            await context.Client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"{context.LanguageDictionary.ConnectedToChat} {connectedChat.Title}! ({connection.Chat})");
        }
    }
}