using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Iris.Api;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Update = Iris.Api.Update;

namespace Iris.Bot
{
    internal class Sender
    {
        private const int MaxMediaCaptionSize = 1024;
        private const ParseMode MessageParseMode = ParseMode.Html;
        private readonly ITelegramBotClient _client;
        private readonly ILogger<Sender> _logger;
        private readonly object _messageBatchLock = new object();

        public Sender(
            ITelegramBotClient client,
            ILogger<Sender> logger)
        {
            _client = client;
            _logger = logger;
        }

        public Task SendAsync(Update update, long chatId)
        {
            Task SendMultipleMediaMessagesAsync()
            {
                SendMultipleMediaMessages(update, chatId);
                return Task.CompletedTask;
            }

            var mediaCount = update.Media.Count();

            switch (mediaCount)
            {
                case 0:
                    return SendTextMessage(update, chatId);
                
                case 1:
                    if (update.FormattedMessage.Length <= MaxMediaCaptionSize)
                    {
                        return SendSingleMediaMessage(update, chatId);
                    }
                    _logger.LogInformation(
                        "Message with one media but more than 1024 characters detected. Sending multiple media messages");
                    return SendMultipleMediaMessagesAsync();
                
                default:
                    return SendMultipleMediaMessagesAsync();
            }
        }

        private Task SendSingleMediaMessage(Update update, long chatId)
        {
            _logger.LogInformation("Sending single message");
            
            List<Media> updateMedia = update.Media.ToList();
            
            return updateMedia.Any(media => media.Type == MediaType.Video) 
                ? SendVideo(update, chatId, updateMedia) 
                : SendPhoto(update, chatId, updateMedia);
        }

        private async Task SendPhoto(Update update, long chatId, List<Media> updateMedia)
        {
            _logger.LogInformation("Sending single photo message");
            
            Media photo = updateMedia.FirstOrDefault(media => media.Type == MediaType.Photo);

            await _client.SendPhotoAsync(
                chatId,
                photo.ToInputOnlineFile(),
                update.FormattedMessage,
                MessageParseMode);
        }

        private async Task SendVideo(Update update, long chatId, List<Media> updateMedia)
        {
            _logger.LogInformation("Sending single video message");
            
            Media video = updateMedia
                .FirstOrDefault(media => media.Type == MediaType.Video);

            await _client.SendVideoAsync(
                chatId,
                video.ToInputOnlineFile(),
                caption: update.FormattedMessage,
                parseMode: MessageParseMode);
        }

        private void SendMultipleMediaMessages(Update update, long chatId)
        {
            lock (_messageBatchLock)
            {
                _logger.LogInformation("Sending media album batch");
            
                Message[] mediaMessages = SendMediaAlbum(update, chatId).Result;
                int? firstMediaMessageId = mediaMessages?.FirstOrDefault()?.MessageId;

                _logger.LogInformation("Sending corresponding message");
            
                SendTextMessage(update, chatId, firstMediaMessageId ?? 0).Wait();                
            }
        }

        private Task SendTextMessage(Update update, long chatId, int replyMessageId = 0)
        {
            _logger.LogInformation("Sending text message");
            
            return _client.SendTextMessageAsync(
                chatId,
                update.FormattedMessage,
                MessageParseMode,
                replyToMessageId: replyMessageId
            );
        }

        private async Task<Message[]> SendMediaAlbum(Update update, long chatId)
        {
            IEnumerable<IAlbumInputMedia> telegramMedia = update.Media
                .Select(media => media.ToAlbumInputMedia());
            
            _logger.LogInformation("Found media. Sending album");

            return await _client.SendMediaGroupAsync(telegramMedia, chatId);
        }
    }
}