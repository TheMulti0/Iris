﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TdLib;

namespace TelegramClient
{
    public class TelegramClient : ITelegramClient
    {
        private readonly TdClient _client;
        private readonly TelegramClientConfig _config;
        private readonly ILogger<TelegramClient> _logger;
        private readonly TdApi.SendMessageOptions _defaultSendMessageOptions = new();
        public IObservable<TdApi.Update> OnUpdateReceived { get; }

        public TelegramClient(
            TdClient client,
            TelegramClientConfig config,
            ILogger<TelegramClient> logger)
        {
            _client = client;
            _config = config;
            _logger = logger;
            OnUpdateReceived = client.OnUpdateReceived();
        }

        public Task<TdApi.FormattedText> ParseTextAsync(string text, TdApi.TextParseMode parseMode)
        {
            _logger.LogDebug("Parse text {}", text);
            return _client.ParseTextEntitiesAsync(text, parseMode);
        }

        public Task<TdApi.Chat> GetChatAsync(long chatId)
        {
            _logger.LogDebug("Get chat {}", chatId);
            return _client.GetChatAsync(chatId);
        }

        public async Task<TdApi.Message> SendMessageAsync(
            long chatId,
            TdApi.InputMessageContent inputMessageContent,
            long replyToMessageId = 0,
            TdApi.ReplyMarkup replyMarkup = null,
            TdApi.SendMessageOptions options = null,
            Action<FileUploadProgress> progress = null,
            CancellationToken token = default)
        {
            _logger.LogDebug("Send message {} to {}", inputMessageContent.Format(), chatId);
            var content = new DisposableMessageContent(inputMessageContent);

            bool hasRemoteStream = inputMessageContent.HasInputFile(out InputRemoteStream file);
            if (hasRemoteStream)
            {
                content = await inputMessageContent.WithInputRemoteStreamAsync(file);
            }

            bool hasRecyclingFile = inputMessageContent.HasInputFile(out InputRecyclingLocalFile r);
            if (hasRecyclingFile)
            {
                content = await inputMessageContent.WithInputRecyclingLocalFileAsync(r);
            }

            return await SendMessageAsync(
                chatId,
                content, replyToMessageId, replyMarkup, options, progress, token);
        }

        private async Task<TdApi.Message> SendMessageAsync(
            long chatId,
            DisposableMessageContent content,
            long replyToMessageId,
            TdApi.ReplyMarkup replyMarkup,
            TdApi.SendMessageOptions options,
            Action<FileUploadProgress> progress,
            CancellationToken token)
        {
            try
            {
                TdApi.Message message = await _client.SendMessageAsync(
                    chatId: chatId,
                    inputMessageContent: content.Content,
                    replyToMessageId: replyToMessageId,
                    replyMarkup: replyMarkup,
                    options: options);

                return await GetMatchingMessageEvents(message, progress)
                    .SelectAwaitWithCancellation(
                        (update, t) =>
                            GetMessageAsync(
                                update,
                                chatId,
                                content.Content,
                                replyToMessageId,
                                replyMarkup,
                                options,
                                t))
                    .FirstOrDefaultAsync(token);
            }
            finally
            {
                await content.DisposeAsync();
            }
        }

        private IAsyncEnumerable<TdApi.Update> GetMatchingMessageEvents(
            TdApi.Message message,
            Action<FileUploadProgress> progress)
        {
            return OnUpdateReceived
                .ToAsyncEnumerable()
                .Timeout(_config.MessageSendTimeout)
                .Where(
                    u =>
                    {
                        switch (u)
                        {
                            case TdApi.Update.UpdateFile f when f.File.Id == message.Id:
                                progress?.Invoke(
                                    new FileUploadProgress(f.File.Remote.UploadedSize, f.File.ExpectedSize));
                                return false;

                            case TdApi.Update.UpdateMessageSendSucceeded m when m.OldMessageId == message.Id:
                                return true;

                            case TdApi.Update.UpdateMessageSendFailed f when f.OldMessageId == message.Id:
                                return true;
                        }

                        return false;
                    });
        }

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
                    _logger.LogDebug("Message send failed {}", f.ErrorMessage);
                    
                    return await HandleMessageSendFailed(
                        f,
                        chatId,
                        inputMessageContent,
                        replyToMessageId,
                        replyMarkup,
                        options,
                        token);
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
            DisposableMessageContent downloadedMessageContent = await GetDownloadStreamMessageContentAsync(url, inputMessageContent);

            return await SendMessageAsync(
                chatId,
                downloadedMessageContent,
                replyToMessageId,
                replyMarkup,
                options,
                null,
                token);
        }

        private static Task<DisposableMessageContent> GetDownloadStreamMessageContentAsync(
            string url,
            TdApi.InputMessageContent inputMessageContent)
        {
            var file = new InputRemoteStream(new RemoteFileStream(url).GetStreamAsync);

            return inputMessageContent.WithInputRemoteStreamAsync(file);
        }

        public async Task<IEnumerable<TdApi.Message>> SendMessageAlbumAsync(
            long chatId,
            TdApi.InputMessageContent[] inputMessageContents,
            long replyToMessageId = 0,
            TdApi.SendMessageOptions options = null,
            CancellationToken token = default)
        {
            _logger.LogDebug("Send message album {} to {}", inputMessageContents.Format(), chatId);
            
            try
            {
                return await SendMessageAlbumUnsafe(
                    chatId,
                    inputMessageContents,
                    replyToMessageId,
                    options,
                    token);
            }
            catch (MessageSendFailedException e)
            {
                _logger.LogDebug("Message album send failed {}, trying to send downloaded message", e.Message);
                
                DisposableMessageContent[] contents = await GetDownloadedMessageContentsAsync(inputMessageContents)
                    .ToArrayAsync(token);

                return await SendMessageAlbumUnsafe(
                    chatId,
                    contents,
                    replyToMessageId,
                    options,
                    token);
            }
        }

        private async Task<IEnumerable<TdApi.Message>> SendMessageAlbumUnsafe(
            long chatId,
            IEnumerable<TdApi.InputMessageContent> inputMessageContents,
            long replyToMessageId,
            TdApi.SendMessageOptions options,
            CancellationToken token)
        {
            options ??= _defaultSendMessageOptions;
            
            var contents = await inputMessageContents
                .ToAsyncEnumerable()
                .SelectAwait(ExtractFiles)
                .ToArrayAsync(token);

            return await SendMessageAlbumUnsafe(chatId, contents, replyToMessageId, options, token);
        }

        private async Task<IEnumerable<TdApi.Message>> SendMessageAlbumUnsafe(
            long chatId,
            DisposableMessageContent[] contents,
            long replyToMessageId,
            TdApi.SendMessageOptions options,
            CancellationToken token)
        {
            try
            {
                TdApi.InputMessageContent[] messageContents = contents.Select(c => c.Content)
                    .ToArray();

                _logger.LogDebug(
                    "Send message album unsafe, chatId: {}, inputMessageContents: {}, replyToMessageId: {}, options: {}",
                    chatId,
                    messageContents.Format(),
                    replyToMessageId,
                    options);

                TdApi.Messages messages = await _client.SendMessageAlbumAsync(
                    chatId: chatId,
                    inputMessageContents: messageContents,
                    replyToMessageId: replyToMessageId,
                    options: options);

                return await GetSentMessages(messages, token)
                    .ToListAsync(token);
            }
            finally
            {
                foreach (DisposableMessageContent content in contents)
                {
                    await content.DisposeAsync();
                }
            }
        }

        private static async ValueTask<DisposableMessageContent> ExtractFiles(TdApi.InputMessageContent content)
        {
            if (content.HasInputFile(out InputRemoteStream file))
            {
                return await content.WithInputRemoteStreamAsync(file);
            }
            if (content.HasInputFile(out InputRecyclingLocalFile r))
            {
                return await content.WithInputRecyclingLocalFileAsync(r);
            }

            return new DisposableMessageContent(content);
        }

        private static async IAsyncEnumerable<DisposableMessageContent> GetDownloadedMessageContentsAsync(
            IEnumerable<TdApi.InputMessageContent> inputMessageContents)
        {
            foreach (TdApi.InputMessageContent inputMessageContent in inputMessageContents)
            {
                if (inputMessageContent.HasFile(out TdApi.InputFile file) &&
                    file.HasUrl(out string url) &&
                    inputMessageContent is TdApi.InputMessageContent.InputMessageVideo)
                {
                    yield return await GetDownloadStreamMessageContentAsync(url, inputMessageContent);
                }
                else
                {
                    yield return new DisposableMessageContent(inputMessageContent);
                }
            }
        }

        private async IAsyncEnumerable<TdApi.Message> GetSentMessages(
            TdApi.Messages messages,
            [EnumeratorCancellation] CancellationToken token)
        {
            var sentMessagesCount = 0;

            await foreach (TdApi.Update update in OnUpdateReceived
                .ToAsyncEnumerable()
                .Timeout(_config.MessageSendTimeout)
                .WithCancellation(token))
            {
                switch (update)
                {
                    case TdApi.Update.UpdateMessageSendFailed f:
                    {
                        throw new MessageSendFailedException(f.ErrorMessage);
                    }

                    case TdApi.Update.UpdateMessageSendSucceeded m
                        when messages.Messages_.All(message => message.Id != m.OldMessageId):
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
                _logger.LogDebug("Trying to send downloaded message");
                
                return await SendDownloadedMessage(
                    chatId,
                    url,
                    inputMessageContent,
                    replyToMessageId,
                    replyMarkup,
                    options,
                    token);
            }

            throw new IndexOutOfRangeException();
        }

        private static void ThrowMessageSendFailed(TdApi.Update.UpdateMessageSendFailed update)
        {
            throw new MessageSendFailedException(update.ErrorMessage);
        }
    }
}