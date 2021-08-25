using System.Collections.Generic;
using System.Threading;
using Common;
using Scraper.Net;
using Telegram.Bot.Types;

namespace TelegramSender
{
    public record MessageInfo(
        string Message,
        IEnumerable<IMediaItem> MediaItems,
        ChatId ChatId,
        CancellationToken CancellationToken = default,
        int ReplyToMessageId = 0,
        bool DownloadMedia = false,
        bool DisableWebPagePreview = true)
    {
        public bool FitsInOneTextMessage => Message.Length <= TelegramConstants.MaxTextMessageLength;

        public bool FitsInOneMediaMessage => Message.Length <= TelegramConstants.MaxMediaCaptionLength;
    }
}