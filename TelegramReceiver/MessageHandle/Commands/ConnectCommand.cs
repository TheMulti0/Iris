using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramReceiver.Data;
using UserDataLayer;
using User = Common.User;

namespace TelegramReceiver
{
    internal class ConnectCommand : ICommand
    {
        private readonly IConnectionsRepository _repository;

        public ITrigger[] Triggers { get; } = {
            new MessageStartsWithTextTrigger("/connect")
        };

        public ConnectCommand(
            IConnectionsRepository repository)
        {
            _repository = repository;
        }

        public async Task OperateAsync(Context context)
        {
            (ITelegramBotClient client, IObservable<Update> incoming, Update currentUpdate) = context;

            Message message = currentUpdate.Message;
            string[] arguments = message.Text.Split(' ');

            if (arguments.Length <= 1)
            {
                await client.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Supply chat id!");
                return;
            }

            var chatId = (ChatId) arguments[1];
            Chat chat;
            try
            {
                chat = await client.GetChatAsync(chatId);
            }
            catch (ChatNotFoundException)
            {
                await client.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Failed to find chat! (Is the bot inside this chat?)");
                return;
            }

            await _repository.AddOrUpdateAsync(message.From, chatId);

            await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"Connected to {chat.Title}! ({chatId})");
        }
    }
}