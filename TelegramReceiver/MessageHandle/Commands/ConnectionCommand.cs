using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramReceiver.Data;

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
            (ITelegramBotClient client, _, Update currentUpdate) = context;

            Message message = currentUpdate.Message;
            ChatId connectedChatId = await _repository.GetAsync(message.From);
            Chat connectedChat = await client.GetChatAsync(connectedChatId);

            if (connectedChat.Type == ChatType.Private)
            {
                await client.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: $"Not connected to any external chat");
                return;
            }
            
            await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"Connected to {connectedChat.Title}! ({connectedChatId})");
        }
    }
}