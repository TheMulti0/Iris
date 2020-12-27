using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
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
            ChatId connectedChat = await _repository.GetAsync(message.From);

            await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"Connected to {(await client.GetChatAsync(connectedChat)).Title}! ({connectedChat})");
        }
    }
}