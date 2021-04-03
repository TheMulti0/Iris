using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TdLib;

namespace TelegramClient
{
    public class TelegramClient : ITelegramClient
    {
        private readonly TdClient _client;
        public IObservable<TdApi.Update> OnUpdateReceived { get; }

        public TelegramClient(TdClient client)
        {
            _client = client;
            OnUpdateReceived = client.OnUpdateReceived();
        }

        public Task<TdApi.Chat> GetChatAsync(long chatId)
        {
            return _client.GetChatAsync(chatId);
        }

        public async Task<TdApi.Message> SendMessageAsync(
            long chatId,
            TdApi.InputMessageContent inputMessageContent,
            long replyToMessageId = 0,
            TdApi.ReplyMarkup replyMarkup = null,
            TdApi.SendMessageOptions options = null,
            CancellationToken token = default)
        {
            TdApi.Message message = await _client.SendMessageAsync(
                chatId: chatId,
                inputMessageContent: inputMessageContent,
                replyToMessageId: replyToMessageId,
                replyMarkup: replyMarkup,
                options: options);
            
            return await GetMatchingMessageEvents(message)
                .SelectAwaitWithCancellation(
                    (update, t) => GetMessageAsync(update, chatId, inputMessageContent, t))
                .FirstOrDefaultAsync(token);
        }

        private IAsyncEnumerable<TdApi.Update> GetMatchingMessageEvents(TdApi.Message message) => OnUpdateReceived
            .ToAsyncEnumerable()
            .Where(u =>
            {
                switch (u)
                {
                    case TdApi.Update.UpdateMessageSendSucceeded m when m.OldMessageId == message.Id:
                        return true;

                    case TdApi.Update.UpdateMessageSendFailed f when f.OldMessageId == message.Id:
                        return true;
                }
                    
                return false;
            });

        private async ValueTask<TdApi.Message> GetMessageAsync(
            TdApi.Update update,
            long chatId,
            TdApi.InputMessageContent inputMessageContent,
            CancellationToken token)
        {
            switch (update)
            {
                case TdApi.Update.UpdateMessageSendSucceeded m:
                    return m.Message;

                case TdApi.Update.UpdateMessageSendFailed f:
                {
                    if (f.ErrorMessage != "Wrong file identifier/HTTP URL specified" &&
                        f.ErrorMessage != "Failed to get HTTP URL content")
                    {
                        throw new MessageSendFailedException(f.ErrorMessage);
                    }

                    if (inputMessageContent.HasFile(out TdApi.InputFile file) &&
                        file.HasUrl(out string url))
                    {
                        return await SendDownloadedMessage(chatId, inputMessageContent, url, token);
                    }

                    break;
                }
            }

            throw new IndexOutOfRangeException();
        }

        private async Task<TdApi.Message> SendDownloadedMessage(
            long chatId,
            TdApi.InputMessageContent inputMessageContent,
            string url,
            CancellationToken token)
        {
            var downloader = new FileDownloader(url);
            TdApi.InputFile downloadedFile = await downloader.DownloadFileAsync();

            TdApi.Message message = await SendMessageAsync(
                chatId,
                inputMessageContent.WithFile(downloadedFile),
                token: token);

            await downloader.DisposeAsync();
            return message;
        }

        public async Task<IEnumerable<TdApi.Message>> SendMessageAlbumAsync(
            long chatId,
            TdApi.InputMessageContent[] inputMessageContents,
            long replyToMessageId = 0,
            TdApi.SendMessageOptions options = null,
            CancellationToken token = default)
        {
            TdApi.Messages messages = await _client.SendMessageAlbumAsync(
                chatId: chatId,
                inputMessageContents: inputMessageContents,
                replyToMessageId: replyToMessageId,
                options: options);

            return await GetSentMessages(messages, token).ToListAsync(token);
        }

        private async IAsyncEnumerable<TdApi.Message> GetSentMessages(
            TdApi.Messages messages,
            [EnumeratorCancellation] CancellationToken token)
        {
            var sentMessagesCount = 0;
            
            await foreach (TdApi.Update update in OnUpdateReceived.ToAsyncEnumerable().WithCancellation(token))
            {
                switch (update)
                {
                    case TdApi.Update.UpdateMessageSendFailed f:
                        throw new MessageSendFailedException(f.ErrorMessage);
                    
                    case TdApi.Update.UpdateMessageSendSucceeded m when messages.Messages_.All(message => message.Id != m.OldMessageId):
                        continue;
                    
                    case TdApi.Update.UpdateMessageSendSucceeded m:
                    {
                        yield return m.Message;

                        sentMessagesCount++;

                        if (messages.TotalCount == sentMessagesCount)
                        {
                            yield break;
                        }
                        break;
                    }
                }
            }
        }
    }
}