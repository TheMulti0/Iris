using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Message = Telegram.Bot.Types.Message;
using Video = Common.Video;

namespace TelegramSender
{
    public class MediaSender
    {
        private readonly HttpClient _httpClient;
        private readonly ITelegramBotClient _client;
        private readonly TextSender _textSender;
        private readonly SemaphoreSlim _messageBatchLock = new(1, 1);

        public MediaSender(
            ITelegramBotClient client,
            TextSender textSender)
        {
            _httpClient = new HttpClient();
            _client = client;
            _textSender = textSender;
        }

        public async Task SendAsync(MessageInfo message)
        {
            async Task HandleException(Exception e)
            {
                if (!message.DownloadMedia)
                {
                    await UploadMedia(message);
                }
            }

            // Make sure no other updates will be sent as message batches until this batch is complete sending
            await _messageBatchLock.WaitAsync();

            try
            {
                await SendUnsafeAsync(message);
            }
            catch (ApiRequestException e)
            {
                if (e.Message == "Bad Request: failed to get HTTP URL content" ||
                    e.Message == "Bad Request: wrong file identifier/HTTP URL specified")
                {
                    await HandleException(e);
                }
                
                throw;
            }
            finally
            {
                // Release the thread even if the operation fails (avoid a deadlock)
                _messageBatchLock.Release();
            }
        }

        private async Task UploadMedia(MessageInfo message)
        {
            // Send media as stream (upload) instead of sending the url of the media

            await SendUnsafeAsync(message with { DownloadMedia = true });
        }

        private async Task SendUnsafeAsync(MessageInfo message)
        {
            List<IAlbumInputMedia> telegramMedia = await message.Media
                .ToAsyncEnumerable()
                .SelectAwait(media => ToAlbumInputMediaAsync(message, media))
                .ToListAsync(message.CancellationToken);

            Task sendTask = message.FitsInOneMediaMessage
                ? SendMediaAlbumWithCaption(message, telegramMedia)
                : SendMediaAlbumWithAdditionalTextMessage(message, telegramMedia);

            await sendTask;
        }

        private async ValueTask<IAlbumInputMedia> ToAlbumInputMediaAsync(MessageInfo message, IMedia media)
        {
            InputMedia inputMedia = await GetInputMediaAsync(message, media);

            switch (media)
            {
                case Video v:
                    var video = new InputMediaVideo(inputMedia)
                    {
                        SupportsStreaming = true
                    };

                    if (v.ThumbnailUrl != null)
                    {
                        video.Thumb = new InputMedia(
                            await _httpClient.GetStreamAsync(v.ThumbnailUrl, message.CancellationToken),
                            "Thumbnail");
                    }
                    
                    if (v.Duration?.Seconds != null)
                    {
                        video.Duration = (int) v.Duration?.Seconds;
                    }
                    if (v.Width != null)
                    {
                        video.Width = (int) v.Width;
                    }
                    if (v.Height != null)
                    {
                        video.Height = (int) v.Height;
                    }

                    return video;
                
                case BytesPhoto p:
                    return new InputMediaPhoto(new InputMedia(new MemoryStream(p.Bytes), "Photo"));
                
                default:
                    return new InputMediaPhoto(inputMedia);
            }
        }

        private async Task<InputMedia> GetInputMediaAsync(MessageInfo message, IMedia media)
        {
            if (message.DownloadMedia)
            {
                return new InputMedia(
                    await _httpClient.GetStreamAsync(media.Url, message.CancellationToken),
                    media.GetType().Name);
            }
            
            return new InputMedia(media.Url);
        }

        private Task<Message[]> SendMediaAlbumWithCaption(MessageInfo message, IEnumerable<IAlbumInputMedia> telegramMedia)
        {
            IEnumerable<IAlbumInputMedia> mediaList = telegramMedia.ToList();
            
            if (mediaList.FirstOrDefault() is not InputMediaBase b)
            {
                return _client.SendMediaGroupAsync(
                    inputMedia: mediaList,
                    chatId: message.ChatId,
                    cancellationToken: message.CancellationToken);
            }
            
            b.Caption = message.Message;
            b.ParseMode = TelegramConstants.MessageParseMode;

            return _client.SendMediaGroupAsync(
                inputMedia: mediaList,
                chatId: message.ChatId, 
                cancellationToken: message.CancellationToken);
        }

        private async Task SendMediaAlbumWithAdditionalTextMessage(MessageInfo message, IEnumerable<IAlbumInputMedia> telegramMedia)
        {
            var firstMediaMessageId = 0;
            
            if (message.Media.Any())
            {
                firstMediaMessageId = await SendMediaAlbumIfAny(message, telegramMedia);
            }

            if (message.Message.Any())
            {
                MessageInfo newMessage = message with { ReplyToMessageId = firstMediaMessageId };

                await _textSender.SendAsync(newMessage);
            }
        }

        private async Task<int> SendMediaAlbumIfAny(MessageInfo message, IEnumerable<IAlbumInputMedia> telegramMedia)
        {
            Message[] mediaMessages = await _client.SendMediaGroupAsync(
                inputMedia: telegramMedia,
                chatId: message.ChatId,
                cancellationToken: message.CancellationToken);
            
            return mediaMessages.FirstOrDefault()?.MessageId ?? 0;
        }
    }
}