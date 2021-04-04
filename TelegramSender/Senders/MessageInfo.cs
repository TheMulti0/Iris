using System.Collections.Generic;
using System.Threading;
using Common;
using TdLib;
using Telegram.Bot.Types;

namespace TelegramSender
{
    public record MessageInfo1(
        IEnumerable<TdApi.InputMessageContent> Content,
        long ChatId,
        CancellationToken CancellationToken = default,
        int ReplyMessageId = 0,
        bool DownloadMedia = false,
        bool DisableWebPagePreview = true);

    public record ParsedMessageInfo(
        TdApi.FormattedText Text,
        IEnumerable<TdApi.InputMessageContent> Media,
        long ChatId,
        int ReplyToMessageId = 0,
        bool DisableWebPagePreview = true,
        CancellationToken CancellationToken = default)
    {
        public bool FitsInOneTextMessage => Text.Text.Length <= TelegramConstants.MaxTextMessageLength;

        public bool FitsInOneMediaMessage => Text.Text.Length <= TelegramConstants.MaxMediaCaptionLength;
    }
    
    public record MessageInfo(
        string Message,
        IEnumerable<IMedia> Media,
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