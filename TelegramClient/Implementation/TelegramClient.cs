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

        public Task<TdApi.FormattedText> ParseTextAsync(string text, TdApi.TextParseMode parseMode)
        {
            return _client.ParseTextEntitiesAsync(text, parseMode);
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
                    (update, t) => GetMessageAsync(update, chatId, inputMessageContent, replyToMessageId, replyMarkup, options, t))
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
            long replyToMessageId,
            TdApi.ReplyMarkup replyMarkup,
            TdApi.SendMessageOptions options,
            CancellationToken token)
        {
            switch (update)
            {
                case TdApi.Update.UpdateMessageSendSucceeded m:
                    return m.Message;

                case TdApi.Update.UpdateMessageSendFailed f:
                {
                    return await HandleMessageSendFailed(f, chatId, inputMessageContent, replyToMessageId, replyMarkup, options, token);
                }
            }

            throw new IndexOutOfRangeException();
        }

        private async Task<TdApi.Message> SendDownloadedMessage(
            long chatId,
            string url,
            TdApi.InputMessageContent inputMessageContent,
            long replyToMessageId,
            TdApi.ReplyMarkup replyMarkup,
            TdApi.SendMessageOptions options,
            CancellationToken token)
        {
            (RemoteFileStream downloader, TdApi.InputMessageContent downloadedMessageContent) = await DownloadMessageContent(url, inputMessageContent);

            TdApi.Message message = await SendMessageAsync(
                chatId,
                downloadedMessageContent,
                replyToMessageId,
                replyMarkup,
                options,
                token);

            await downloader.DisposeAsync();
            
            return message;
        }

        private static async Task<(RemoteFileStream, TdApi.InputMessageContent)> DownloadMessageContent(
            string url,
            TdApi.InputMessageContent inputMessageContent)
        {
            var downloader = new RemoteFileStream(url);

            TdApi.InputFile downloadedFile = await downloader.GetFileAsync();
            TdApi.InputMessageContent downloadedMessageContent = inputMessageContent.WithFile(downloadedFile);
            
            return (downloader, downloadedMessageContent);
        }

        public async Task<IEnumerable<TdApi.Message>> SendMessageAlbumAsync(
            long chatId,
            TdApi.InputMessageContent[] inputMessageContents,
            long replyToMessageId = 0,
            TdApi.SendMessageOptions options = null,
            CancellationToken token = default)
        {
            try
            {
                return await SendMessageAlbumUnsafe(chatId, inputMessageContents, replyToMessageId, options, false, token);
            }
            catch (MessageSendFailedException e)
            {
                (TdApi.InputMessageContent content, RemoteFileStream downloader)[] contents 
                    = await GetDownloadedMessageContents(inputMessageContents)
                        .ToArrayAsync(token);

                var newInputMessageContents = contents.Select(t => t.content).ToArray();
                
                IEnumerable<TdApi.Message> messages = await SendMessageAlbumUnsafe(
                    chatId,
                    newInputMessageContents,
                    replyToMessageId,
                    options,
                    true,
                    token);
                
                foreach ((TdApi.InputMessageContent _, RemoteFileStream downloader) in contents)
                {
                    await downloader.DisposeAsync();
                }

                return messages;
            }
        }

        private async Task<IEnumerable<TdApi.Message>> SendMessageAlbumUnsafe(
            long chatId,
            TdApi.InputMessageContent[] inputMessageContents,
            long replyToMessageId,
            TdApi.SendMessageOptions options,
            bool ignoreFailure,
            CancellationToken token)
        {
            TdApi.Messages messages = await _client.SendMessageAlbumAsync(
                chatId: chatId,
                inputMessageContents: inputMessageContents,
                replyToMessageId: replyToMessageId,
                options: options);

            return await GetSentMessages(messages, ignoreFailure, token)
                .ToListAsync(token);
        }

        private static async IAsyncEnumerable<(TdApi.InputMessageContent content, RemoteFileStream downloader)> GetDownloadedMessageContents(
            IEnumerable<TdApi.InputMessageContent> inputMessageContents)
        {
            foreach (TdApi.InputMessageContent inputMessageContent in inputMessageContents)
            {
                if (inputMessageContent.HasFile(out TdApi.InputFile file) &&
                    file.HasUrl(out string url))
                {
                    (RemoteFileStream downloader, TdApi.InputMessageContent downloadedMessageContent) =
                        await DownloadMessageContent(url, inputMessageContent);

                    yield return (downloadedMessageContent, downloader);
                }
                else
                {
                    yield return (inputMessageContent, null);
                }
            }
        }

        private async IAsyncEnumerable<TdApi.Message> GetSentMessages(
            TdApi.Messages messages,
            bool ignoreFailure,
            [EnumeratorCancellation] CancellationToken token)
        {
            var sentMessagesCount = 0;
            
            await foreach (TdApi.Update update in OnUpdateReceived.ToAsyncEnumerable().WithCancellation(token))
            {
                switch (update)
                {
                    case TdApi.Update.UpdateMessageSendFailed f when !ignoreFailure:
                    {
                        throw new MessageSendFailedException(f.ErrorMessage);
                    }
                    
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

        private async Task<TdApi.Message> HandleMessageSendFailed(
            TdApi.Update.UpdateMessageSendFailed update,
            long chatId,
            TdApi.InputMessageContent inputMessageContent,
            long replyToMessageId,
            TdApi.ReplyMarkup replyMarkup,
            TdApi.SendMessageOptions options,
            CancellationToken token)
        {
            if (update.ErrorMessage != "Wrong file identifier/HTTP URL specified" &&
                update.ErrorMessage != "Failed to get HTTP URL content")
            {
                ThrowMessageSendFailed(update);
            }

            if (inputMessageContent.HasFile(out TdApi.InputFile file) &&
                file.HasUrl(out string url))
            {
                return await SendDownloadedMessage(chatId, url, inputMessageContent, replyToMessageId, replyMarkup, options, token);
            }
            
            throw new IndexOutOfRangeException();
        }

        private static void ThrowMessageSendFailed(TdApi.Update.UpdateMessageSendFailed update)
        {
            throw new MessageSendFailedException(update.ErrorMessage);
        }
    }
}