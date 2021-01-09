using System;
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
            Message message = context.Trigger.Message;

            var chatAsync = await context.Client.GetChatAsync(context.ConnectedChatId);
            
            if (Equals((ChatId) context.ConnectedChatId, (ChatId) message.Chat.Id))
            {
                await context.Client.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: $"{context.LanguageDictionary.DisconnectedFrom} {chatAsync.Title}! ({context.ConnectedChatId})");   
            }

            await _repository.AddOrUpdateAsync(message.From, context.ContextChatId, context.Language);

            await context.Client.SendTextMessageAsync(
                chatId: context.ContextChatId,
                text: $"{context.LanguageDictionary.DisconnectedFrom} {chatAsync.Title}! ({context.ConnectedChatId})");
        }
    }
}