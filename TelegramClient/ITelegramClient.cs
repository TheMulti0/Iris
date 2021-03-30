using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TdLib;

namespace TelegramClient
{
    public interface ITelegramClient
    {
        IObservable<TdApi.Update> OnUpdateReceived { get; }

        Task<TdApi.Message> SendMessageAsync(
            long chatId,
            TdApi.InputMessageContent inputMessageContent,
            long replyToMessageId = 0,
            TdApi.ReplyMarkup replyMarkup = null,
            TdApi.SendMessageOptions options = null);

        Task<IEnumerable<TdApi.Message>> SendMessagesAsync(
            long chatId,
            TdApi.InputMessageContent[] inputMessageContents,
            long replyToMessageId = 0,
            TdApi.SendMessageOptions options = null);
    }
}