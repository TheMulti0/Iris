using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using TelegramReceiver.Data;

namespace TelegramReceiver
{
    internal class DisconnectCommand : ICommand
    {
        private readonly IConnectionsRepository _repository;

        public ITrigger[] Triggers { get; } = {
            new MessageStartsWithTextTrigger("/disconnect")
        };

        public DisconnectCommand(
            IConnectionsRepository repository)
        {
            _repository = repository;
        }

        public async Task OperateAsync(Context context)
        {
            (ITelegramBotClient client, _, Update currentUpdate) = context;

            Message message = currentUpdate.Message;
            var connectedChat = await _repository.GetAsync(message.From);

            if (connectedChat == null ||
                Equals((ChatId) connectedChat, (ChatId) message.Chat.Id))
            {
                await client.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: $"Disconnected from {(await client.GetChatAsync(connectedChat)).Title}! ({connectedChat})");   
            }

            await _repository.AddOrUpdateAsync(message.From, message.Chat.Id);

            await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"Disconnected from {(await client.GetChatAsync(connectedChat)).Title}! ({connectedChat})");
        }
    }
}