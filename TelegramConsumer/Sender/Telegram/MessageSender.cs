using System.Collections.Generic;
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
        private const int MaxTextMessageLength = 4096;
        private const int MaxMediaCaptionLength = 1024;
        private const ParseMode MessageParseMode = ParseMode.Html;
        private const bool DisableWebPagePreview = true;

        private readonly ILogger<MessageSender> _logger;
        private readonly SemaphoreSlim _messageBatchLock = new SemaphoreSlim(1, 1);

        public MessageSender(ILogger<MessageSender> logger)
        {
            _logger = logger;
        }

        private static bool CanUpdateFitInOneMediaMessage(UpdateMessage update)
            => update.Message.Length <= MaxMediaCaptionLength;

        public Task SendAsync(
            ITelegramBotClient client,
            UpdateMessage update,
            ChatId chatId)
        {
            switch (update.Media?.Length ?? 0)
            {
                case 0:
                    return SendTextMessage(client, update, chatId);

                case 1 when CanUpdateFitInOneMediaMessage(update):
                    return SendSingleMediaMessage(client, update, chatId);

                default:
                    return SendMediaMessageBatch(client, update, chatId);
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

        private Task SendMediaMessageBatchUnsafe(
            ITelegramBotClient client,
            UpdateMessage update,
            ChatId chatId)
        {
            _logger.LogInformation("Sending media album batch");

            if (CanUpdateFitInOneMediaMessage(update))
            {
                return SendMediaAlbumWithCaption(client, update, chatId);
            }
            else
            {
                return SendMediaAlbumAndCorrespondingMessage(client, update, chatId);
            }
        }

        private Task<Message[]> SendMediaAlbumWithCaption(
            ITelegramBotClient client,
            UpdateMessage update,
            ChatId chatId)
        {
            IAlbumInputMedia ToAlbumInputMedia(Media media, int index)
            {
                return index > 0
                    ? media.ToAlbumInputMedia()
                    : media.ToAlbumInputMedia(update.Message, MessageParseMode);
            }

            IEnumerable<IAlbumInputMedia> telegramMedia = update.Media
                .Select(ToAlbumInputMedia);

            _logger.LogInformation("Sending media album with caption");

            return client.SendMediaGroupAsync(telegramMedia, chatId);
        }

        private async Task SendMediaAlbumAndCorrespondingMessage(
            ITelegramBotClient client,
            UpdateMessage update,
            ChatId chatId)
        {
            Message[] mediaMessages = await SendMediaAlbum(client, update, chatId);

            if (update.Message.Any())
            {
                int? firstMediaMessageId = mediaMessages?.FirstOrDefault()
                    ?.MessageId;

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