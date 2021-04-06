using System.Collections.Generic;
using System.Threading;
using TdLib;

namespace TelegramSender
{
    public record ParsedMessageInfo(
        TdApi.FormattedText Text,
        IEnumerable<TdApi.InputMessageContent> Media,
        long ChatId,
        long ReplyToMessageId = 0,
        bool DisableWebPagePreview = true,
        CancellationToken CancellationToken = default)
    {
        public bool FitsInOneTextMessage => Text.Text.Length <= TelegramConstants.MaxTextMessageLength;

        public bool FitsInOneMediaMessage => Text.Text.Length <= TelegramConstants.MaxMediaCaptionLength;
    }
}