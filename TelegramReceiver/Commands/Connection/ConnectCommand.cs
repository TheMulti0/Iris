﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Message = Telegram.Bot.Types.Message;

namespace TelegramReceiver
{
    internal class ConnectCommand : BaseCommand, ICommand
    {
        private readonly IConnectionsRepository _repository;

        public ConnectCommand(
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
                return new NoRedirectResult();
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
                return new NoRedirectResult();
            }
            
            ChatMember[] administrators = await Client.GetChatAdministratorsAsync(chatId, token);

            if (administrators.All(member => member.User.Id != message.From.Id))
            {
                await Client.SendTextMessageAsync(
                    chatId: ContextChat,
                    text: Dictionary.NotAdmin,
                    cancellationToken: token);
                return new NoRedirectResult();
            }

            Connection.ChatId = chat.Id; 
            await _repository.AddOrUpdateAsync(message.From, Connection);

            return new RedirectResult(Route.Connection, Context with { Connection = Connection, ConnectedChat = chat });
        }
    }
}