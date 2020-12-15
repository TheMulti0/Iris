using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Video = Common.Video;

namespace TelegramBot
{
    public class MediaSender
    {
        private readonly HttpClient _httpClient;
        private readonly ITelegramBotClient _client;
        private readonly TextSender _textSender;
        private readonly ILogger<MediaSender> _logger;
        private readonly SemaphoreSlim _messageBatchLock = new SemaphoreSlim(1, 1);
        private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions { WriteIndented = true };

        public MediaSender(
            ITelegramBotClient client,
            TextSender textSender,
            ILogger<MediaSender> logger)
        {
            _httpClient = new HttpClient();
            _client = client;
            _textSender = textSender;
            _logger = logger;
        }

        public async Task SendAsync(MessageInfo message)
        {
            async Task HandleException(Exception e)
            {
                _logger.LogError(
                    e,
                    "Failed to send media \n {} \n {}",
                    JsonSerializer.Serialize(message.Media, _serializerOptions),
                    e);

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
                await HandleException(e);
            }
            catch (IOException e)
            {
                await HandleException(e);
            }
            finally
            {
                // Release the thread even if the operation fails (avoid a deadlock)
                _logger.LogInformation("Releasing message batch lock");
                _messageBatchLock.Release();
            }
        }

        private async Task UploadMedia(MessageInfo message)
        {
            _logger.LogInformation("Retrying with DownloadMedia set to true");

            // Send media as stream (upload) instead of sending the url of the media

            await SendUnsafeAsync(message with { DownloadMedia = true });
        }

        private async Task SendUnsafeAsync(MessageInfo message)
        {
            foreach (var m in message.Media)
            {
                if (m is not Video)
                {
                    continue;
                }
                
                Console.WriteLine("Video");
                Console.WriteLine(m.Url);
            }
            
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
            _logger.LogInformation("Sending media album with caption");

            if (telegramMedia.FirstOrDefault() is InputMediaBase b)
            {
                b.Caption = message.Message;
                b.ParseMode = TelegramConstants.MessageParseMode;
            }

            return _client.SendMediaGroupAsync(
                inputMedia: telegramMedia,
                chatId: message.ChatId, 
                cancellationToken: message.CancellationToken);
        }

        private async Task SendMediaAlbumWithAdditionalTextMessage(MessageInfo message, IEnumerable<IAlbumInputMedia> telegramMedia)
        {
            _logger.LogInformation("Sending media album with additional text message");
            
            var firstMediaMessageId = 0;
            
            if (message.Media.Any())
            {
                firstMediaMessageId = await SendMediaAlbumIfAny(message, telegramMedia);
            }

            if (message.Message.Any())
            {
                MessageInfo newMessage = message with { ReplyMessageId = firstMediaMessageId };

                await _textSender.SendAsync(newMessage);
            }
        }

        private async Task<int> SendMediaAlbumIfAny(MessageInfo message, IEnumerable<IAlbumInputMedia> telegramMedia)
        {
            _logger.LogInformation("Sending media album");
            
            Message[] mediaMessages = await _client.SendMediaGroupAsync(
                inputMedia: telegramMedia,
                chatId: message.ChatId,
                cancellationToken: message.CancellationToken);
            
            return mediaMessages.FirstOrDefault()?.MessageId ?? 0;
        }
    }
}