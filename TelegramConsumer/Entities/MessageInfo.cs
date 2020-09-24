using System.Threading;
using Telegram.Bot.Types;
using UpdatesConsumer;

namespace TelegramConsumer
{
    public class MessageInfo
    {
        public string Message { get; }

        public IMedia[] Media { get; }

        public ChatId ChatId { get; }
        
        public CancellationToken CancellationToken { get; }

        public int ReplyMessageId { get; }
        
        public bool DownloadMedia { get; set; }

        public bool FitsInOneTextMessage { get; }

        public bool FitsInOneMediaMessage { get; }

        public MessageInfo(
            string message,
            IMedia[] media,
            ChatId chatId,
            CancellationToken cancellationToken = default,
            int replyMessageId = 0,
            bool downloadMedia = false)
        {
            Message = message;
            Media = media;
            ChatId = chatId;
            CancellationToken = cancellationToken;
            ReplyMessageId = replyMessageId;
            DownloadMedia = downloadMedia;
            
            FitsInOneTextMessage = Message.Length <= TelegramConstants.MaxTextMessageLength;
            FitsInOneMediaMessage = Message.Length <= TelegramConstants.MaxMediaCaptionLength;            
        }
    }
}