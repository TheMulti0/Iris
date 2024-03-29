﻿using System.Threading;
using System.Threading.Tasks;
using Common;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using ChatType = Telegram.Bot.Types.Enums.ChatType;
using Message = Telegram.Bot.Types.Message;

namespace TelegramReceiver
{
    internal class ConnectionCommand : BaseCommand, ICommand
    {
        public ConnectionCommand(Context context): base(context)
        {
        }

        public async Task<IRedirectResult> ExecuteAsync(CancellationToken token)
        {
            Chat connectedChatInfo = Context.ConnectedChat ?? await Client.GetChatAsync(ConnectedChat, token);

            if (connectedChatInfo.Type == ChatType.Private)
            {
                await Client.SendTextMessageAsync(
                    chatId: ContextChat,
                    text: Dictionary.NotConnected,
                    cancellationToken: token);
                
                return new NoRedirectResult();
            }
            
            await Client.SendTextMessageAsync(
                chatId: ContextChat,
                text: $"{Dictionary.ConnectedToChat} {GetChatTitle(connectedChatInfo)}",
                cancellationToken: token);
            
            return new NoRedirectResult();
        }
    }
}