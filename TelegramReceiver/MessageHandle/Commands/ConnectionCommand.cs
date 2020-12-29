using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Message = Telegram.Bot.Types.Message;

namespace TelegramReceiver
{
    internal class ConnectionCommand : ICommand
    {
        public ITrigger[] Triggers { get; } = {
            new MessageStartsWithTextTrigger("/connection")
        };

        public async Task OperateAsync(Context context)
        {
            Message message = context.Update.Message;
            Chat connectedChat = await context.Client.GetChatAsync(context.ConnectedChatId);

            if (connectedChat.Type == ChatType.Private)
            {
                await context.Client.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: context.LanguageDictionary.NotConnected);
                return;
            }
            
            await context.Client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"{context.LanguageDictionary.ConnectedToChat} {connectedChat.Title}! ({context.ConnectedChatId})");
        }
    }
}