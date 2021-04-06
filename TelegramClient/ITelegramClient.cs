using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TdLib;

namespace TelegramClient
{
    public interface ITelegramClient
    {
        IObservable<TdApi.Update> OnUpdateReceived { get; }

        Task<TdApi.FormattedText> ParseTextAsync(string text, TdApi.TextParseMode parseMode);

        Task<TdApi.Chat> GetChatAsync(long chatId);

        Task<TdApi.Message> SendMessageAsync(
            long chatId,
            TdApi.InputMessageContent inputMessageContent,
            long replyToMessageId = 0,
            TdApi.ReplyMarkup replyMarkup = null,
            TdApi.SendMessageOptions options = null,
            CancellationToken token = default);

        Task<IEnumerable<TdApi.Message>> SendMessageAlbumAsync(
            long chatId,
            TdApi.InputMessageContent[] inputMessageContents,
            long replyToMessageId = 0,
            TdApi.SendMessageOptions options = null,
            CancellationToken token = default);
    }
}