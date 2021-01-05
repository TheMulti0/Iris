using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Common;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramReceiver.Data;
using UserDataLayer;
using Message = Telegram.Bot.Types.Message;
using Update = Telegram.Bot.Types.Update;
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
            Message message = context.Update.Message;
            string[] arguments = message.Text.Split(' ');

            if (arguments.Length <= 1)
            {
                await context.Client.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: context.LanguageDictionary.NoChatId);
                return;
            }

            var chatId = (ChatId) arguments[1];
            Chat chat;
            try
            {
                chat = await context.Client.GetChatAsync(chatId);
            }
            catch (ChatNotFoundException)
            {
                await context.Client.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: context.LanguageDictionary.NoChat);
                return;
            }
            
            ChatMember[] administrators = await context.Client.GetChatAdministratorsAsync(chatId);

            if (administrators.All(member => member.User.Id != message.From.Id))
            {
                await context.Client.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: context.LanguageDictionary.NotAdmin);
                return;
            }

            await _repository.AddOrUpdateAsync(message.From, chatId, context.Language);

            string chatTitle = chat.Title != null 
                ? $" {chat.Title}" 
                : string.Empty;
            
            await context.Client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"{context.LanguageDictionary.ConnectedToChat}{chatTitle}! ({chatId})");
        }
    }
}