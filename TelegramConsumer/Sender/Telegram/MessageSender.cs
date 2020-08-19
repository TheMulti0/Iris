using System;
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

        private readonly ITelegramBotClient _client;
        private readonly ILogger<MessageSender> _logger;
        private readonly SemaphoreSlim _messageBatchLock = new SemaphoreSlim(1, 1);

        public MessageSender(
            ITelegramBotClient client,
            ILogger<MessageSender> logger)
        {
            _client = client;
            _logger = logger;
        }

        public Task SendAsync(
            UpdateMessage update,
            ChatId chatId)
        {
            bool canUpdateFitInOneTextMessage = update.Message.Length <= MaxTextMessageLength;
            bool canUpdateFitInOneMediaMessage = update.Message.Length <= MaxMediaCaptionLength;

            _logger.LogWarning(
                "Message length: {}",
                update.Message.Length);

            switch (update.Media?.Length ?? 0)
            {
                // Only when there is no media in the update, and the update's content can fit in one Telegram message
                case 0 when canUpdateFitInOneTextMessage:
                    return SendSingleTextMessage(update, chatId);

                // Only if there is 1 media item, and the update's content can fit as a media caption (in one Telegram message)
                case 1 when canUpdateFitInOneMediaMessage:
                    return SendSingleMediaMessage(update, chatId);

                // Either when:
                // 1. When there is more than 1 media items,
                // 2. When the update's content cannot fit in a single message (text message / single media message)
                default:
                    return SendMessageBatch(update, chatId, canUpdateFitInOneTextMessage, canUpdateFitInOneMediaMessage);
            }
        }

        private Task<Message> SendSingleTextMessage(
            UpdateMessage update,
            ChatId chatId,
            int replyMessageId = default)
        {
            _logger.LogInformation("Sending text message");

            return _client.SendTextMessageAsync(
                chatId: chatId,
                text: update.Message,
                parseMode: MessageParseMode,
                disableWebPagePreview: DisableWebPagePreview,
                replyToMessageId: replyMessageId
                );
        }

        private async Task SendMultipleTextMessages(
            UpdateMessage update,
            ChatId chatId,
            int firstReplyMessageId = default)
        {
            _logger.LogInformation("Sending multiple text messages");

            string text = update.Message;
            int textLength = text.Length;
            if (textLength > MaxTextMessageLength)
            {
                IEnumerable<string> messageChunks = ChunkifyText(
                    text,
                    MaxTextMessageLength,
                    "\n>>>",
                    '\n',
                    ',',
                    '.');

                int lastMessageId = firstReplyMessageId;

                foreach (string message in messageChunks)
                {
                    var newUpdateMessage = new UpdateMessage
                    {
                        Media = update.Media,
                        Message = message
                    };

                    var lastMessage = await SendSingleTextMessage(
                        newUpdateMessage,
                        chatId,
                        lastMessageId);

                    lastMessageId = lastMessage.MessageId;
                }
            }
        }

        private IEnumerable<string> ChunkifyText(
            string bigString,
            int maxLength,
            string suffix,
            params char[] punctuation)
        {
            var chunks = new List<string>();

            int index = 0;
            var startIndex = 0;

            int bigStringLength = bigString.Length;
            while (startIndex < bigStringLength)
            {
                if (index == bigStringLength - 1)
                {
                    suffix = "";
                }
                maxLength -= suffix.Length;

                string chunk = startIndex + maxLength >= bigStringLength
                    ? bigString.Substring(startIndex)
                    : bigString.Substring(startIndex, maxLength);

                int endIndex = chunk.LastIndexOfAny(punctuation);

                if (endIndex < 0)
                    endIndex = chunk.LastIndexOf(" ", StringComparison.Ordinal);

                if (endIndex < 0)
                    endIndex = Math.Min(maxLength - 1, chunk.Length - 1);

                chunks.Add(chunk.Substring(0, endIndex + 1) + suffix);

                index++;
                startIndex += endIndex + 1;
            }

            return chunks;
        }

        private Task SendSingleMediaMessage(
            UpdateMessage update,
            ChatId chatId)
        {
            return update.Media.Any(media => media.Type == MediaType.Video)
                ? SendVideo(update, chatId)
                : SendPhoto(update, chatId);
        }

        private Task SendPhoto(
            UpdateMessage update,
            ChatId chatId)
        {
            _logger.LogInformation("Sending single photo message");

            var photo = update.Media.FirstOrDefault(media => media.Type == MediaType.Photo);

            return _client.SendPhotoAsync(
                chatId: chatId,
                photo: photo.ToInputOnlineFile(),
                caption: update.Message,
                parseMode: MessageParseMode);
        }

        private Task SendVideo(
            UpdateMessage update,
            ChatId chatId)
        {
            _logger.LogInformation("Sending single video message");

            var video = update.Media.FirstOrDefault(media => media.Type == MediaType.Video);

            return _client.SendVideoAsync(
                chatId: chatId,
                video: video.ToInputOnlineFile(),
                caption: update.Message,
                parseMode: MessageParseMode);
        }

        private async Task SendMessageBatch(
            UpdateMessage update,
            ChatId chatId,
            bool canUpdateFitInOneTextMessage,
            bool canUpdateFitInOneMediaMessage)
        {
            // Make sure no other updates will be sent as message batches until this batch is complete sending
            await _messageBatchLock.WaitAsync();

            try
            {
                await SendMessageBatchUnsafe(update, chatId, canUpdateFitInOneTextMessage, canUpdateFitInOneMediaMessage);
            }
            finally
            {
                // Release the thread even if the operation fails (avoid a deadlock)
                _messageBatchLock.Release();
            }
        }

        private async Task SendMessageBatchUnsafe(
            UpdateMessage update,
            ChatId chatId,
            bool canUpdateFitInOneTextMessage,
            bool canUpdateFitInOneMediaMessage)
        {
            _logger.LogInformation("Sending message batch");
            
            if (canUpdateFitInOneMediaMessage)
            {
                await SendMediaAlbumWithCaption(update, chatId);
                return;
            }

            int firstMediaMessageId = await SendMediaAlbumIfAny(update, chatId);

            await SendTextMessagesIfAny(
                update,
                chatId,
                firstMediaMessageId,
                canUpdateFitInOneTextMessage);
        }

        private async Task<int> SendMediaAlbumIfAny(UpdateMessage update, ChatId chatId)
        {
            bool hasMedia = update.Media?.Any() ?? false;
            if (!hasMedia)
            {
                return 0;
            }
            
            Message[] mediaMessages = await SendMediaAlbum(update, chatId);
            return mediaMessages.FirstOrDefault()?.MessageId ?? 0;
        }

        private async Task SendTextMessagesIfAny(
            UpdateMessage update,
            ChatId chatId,
            int firstMediaMessageId,
            bool canUpdateFitInOneTextMessage)
        {
            if (update.Message.Any())
            {
                _logger.LogInformation("Sending corresponding messages");

                if (canUpdateFitInOneTextMessage)
                {
                    await SendSingleTextMessage(update, chatId, firstMediaMessageId);
                }
                else
                {
                    await SendMultipleTextMessages(update, chatId, firstMediaMessageId);
                }
            }
        }

        private Task<Message[]> SendMediaAlbumWithCaption(
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

            return _client.SendMediaGroupAsync(telegramMedia, chatId);
        }

        private Task<Message[]> SendMediaAlbum(
            UpdateMessage update,
            ChatId chatId)
        {
            var telegramMedia = update.Media
                .Select(media => media.ToAlbumInputMedia());

            _logger.LogInformation("Sending media album");

            return _client.SendMediaGroupAsync(telegramMedia, chatId);
        }
    }
}