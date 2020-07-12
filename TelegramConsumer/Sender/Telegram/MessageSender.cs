using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramConsumer
{
    public class MessageSender
    {
        private const int MaxMediaCaptionSize = 1024;
        private const ParseMode MessageParseMode = ParseMode.Html;
        private const bool DisableWebPagePreview = true;

        private readonly ILogger<MessageSender> _logger;
        private readonly SemaphoreSlim _messageBatchLock = new SemaphoreSlim(1, 1);

        public MessageSender(ILogger<MessageSender> logger)
        {
            _logger = logger;
        }

        public async Task SendAsync(
            ITelegramBotClient client,
            UpdateMessage update,
            ChatId chatId)
        {
            switch (update.Media.Length)
            {
                case 0:
                    await SendTextMessage(client, update, chatId);
                    break;

                case 1 when update.Message.Length <= MaxMediaCaptionSize:
                    await SendSingleMediaMessage(client, update, chatId);
                    break;

                default:
                    await SendMediaMessageBatch(client, update, chatId);
                    break;
            }
        }

        private Task SendTextMessage(
            ITelegramBotClient client,
            UpdateMessage update, 
            ChatId chatId,
            int replyMessageId = default)
        {
            _logger.LogInformation("Sending text message");

            return client.SendTextMessageAsync(
                chatId: chatId,
                text: update.Message,
                parseMode: MessageParseMode,
                disableWebPagePreview: DisableWebPagePreview,
                replyToMessageId: replyMessageId
            );
        }

        private Task SendSingleMediaMessage(
            ITelegramBotClient client,
            UpdateMessage update,
            ChatId chatId)
        {
            return update.Media.Any(media => media.Type == MediaType.Video)
                ? SendVideo(client, update, chatId)
                : SendPhoto(client, update, chatId);
        }

        private Task SendPhoto(
            ITelegramBotClient client,
            UpdateMessage update, 
            ChatId chatId)
        {
            _logger.LogInformation("Sending single photo message");

            var photo = update.Media.FirstOrDefault(media => media.Type == MediaType.Photo);

            return client.SendPhotoAsync(
                chatId: chatId,
                photo: photo.ToInputOnlineFile(),
                caption: update.Message,
                parseMode: MessageParseMode);
        }

        private Task SendVideo(
            ITelegramBotClient client,
            UpdateMessage update,
            ChatId chatId)
        {
            _logger.LogInformation("Sending single video message");

            var video = update.Media.FirstOrDefault(media => media.Type == MediaType.Video);

            return client.SendVideoAsync(
                chatId: chatId,
                video: video.ToInputOnlineFile(),
                caption: update.Message,
                parseMode: MessageParseMode);
        }

        private async Task SendMediaMessageBatch(
            ITelegramBotClient client,
            UpdateMessage update,
            ChatId chatId)
        {
            // Make sure no other updates will be sent as message batches until this batch is complete sending
            await _messageBatchLock.WaitAsync();

            try
            {
                await SendMediaMessageBatchUnsafe(client, update, chatId);
            }
            finally
            {
                // Release the thread even if the operation fails (avoid a deadlock)
                _messageBatchLock.Release();
            }

        }

        private async Task SendMediaMessageBatchUnsafe(
            ITelegramBotClient client,
            UpdateMessage update, 
            ChatId chatId)
        {
            _logger.LogInformation("Sending media album batch");
            var mediaMessages = await SendMediaAlbum(client, update, chatId);

            if (update.Message.Any())
            {
                var firstMediaMessageId = mediaMessages?.FirstOrDefault()?.MessageId;

                _logger.LogInformation("Sending corresponding message");

                await SendTextMessage(client, update, chatId, firstMediaMessageId ?? 0);
            }
        }

        private Task<Message[]> SendMediaAlbum(
            ITelegramBotClient client,
            UpdateMessage update,
            ChatId chatId)
        {
            var telegramMedia = update.Media
                .Select(media => media.ToAlbumInputMedia());

            _logger.LogInformation("Sending media album");

            return client.SendMediaGroupAsync(telegramMedia, chatId);
        }
    }
}