using System.Threading;
using Telegram.Bot.Types;

namespace TelegramConsumer
{
    public class MessageInfo
    {
        public string Message { get; }

        public Media[] Media { get; }

        public ChatId ChatId { get; }
        
        public CancellationToken CancellationToken { get; }

        public int ReplyMessageId { get; }
        
        public bool FitsInOneTextMessage { get; }

        public bool FitsInOneMediaMessage { get; }

        public MessageInfo(
            string message,
            Media[] media,
            ChatId chatId,
            CancellationToken cancellationToken = default,
            int replyMessageId = 0)
        {
            Message = message;
            Media = media;
            ChatId = chatId;
            CancellationToken = cancellationToken;
            ReplyMessageId = replyMessageId;
            
            FitsInOneTextMessage = Message.Length <= TelegramConstants.MaxTextMessageLength;
            FitsInOneMediaMessage = Message.Length <= TelegramConstants.MaxMediaCaptionLength;            
        }
    }
}