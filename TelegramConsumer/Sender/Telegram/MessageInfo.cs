using System.Threading;
using Telegram.Bot.Types;

namespace TelegramConsumer
{
    internal class MessageInfo
    {
        public string Message { get; }

        public Media[] Media { get; }

        public ChatId ChatId { get; }

        public bool FitsInOneTextMessage { get; }

        public bool FitsInOneMediaMessage { get; }

        public int ReplyMessageId { get; }

        public MessageInfo(
            string message,
            Media[] media,
            ChatId chatId,
            int replyMessageId = 0)
        {
            Message = message;
            Media = media;
            ChatId = chatId;
            ReplyMessageId = replyMessageId;
            
            FitsInOneTextMessage = Message.Length <= TelegramConstants.MaxTextMessageLength;
            FitsInOneMediaMessage = Message.Length <= TelegramConstants.MaxMediaCaptionLength;            
        }
    }
}