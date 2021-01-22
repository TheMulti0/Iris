using System.Collections.Generic;
using System.Threading;
using Common;
using Telegram.Bot.Types;

namespace TelegramSender
{
    public record MessageInfo(
        string Message,
        IEnumerable<IMedia> Media,
        ChatId ChatId,
        CancellationToken CancellationToken = default,
        int ReplyMessageId = 0,
        bool DownloadMedia = false,
        bool DisableWebPagePreview = true)
    {
        public bool FitsInOneTextMessage => Message.Length <= TelegramConstants.MaxTextMessageLength;

        public bool FitsInOneMediaMessage => Message.Length <= TelegramConstants.MaxMediaCaptionLength;
    }
}