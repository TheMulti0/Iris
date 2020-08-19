using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using static TelegramConsumer.TelegramConstants;

namespace TelegramConsumer
{
    internal class MessageInfo
    {
        public string Message { get; }

        public Media[] Media { get; }

        public ChatId ChatId { get; }

        public bool FitsInOneTextMessage { get; }

        public bool FitsInOneMediaMessage { get; }

        public int ReplyMessageId { get; }

        public MessageInfo(
            UpdateMessage updateMessage,
            ChatId chatId,
            int replyMessageId = 0
            ) : this(updateMessage.Message,
                     updateMessage.Media,
                     chatId,
                     replyMessageId)
        {
        }

        public MessageInfo(
            string message,
            Media[] media,
            ChatId chatId,
            int replyMessageId = 0)
        {
            Message = message;
            Media = media;
            ChatId = chatId;
            ReplyMessageId = replyMessageId;
            
            FitsInOneTextMessage = Message.Length <= MaxTextMessageLength;
            FitsInOneMediaMessage = Message.Length <= MaxMediaCaptionLength;            
        }
    }
    
    public class MessageSender
    {
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
            UpdateMessage updateMessage,
            ChatId chatId)
        {
            var info = new MessageInfo(updateMessage, chatId);

            _logger.LogWarning(
                "Message length: {}",
                updateMessage.Message.Length);

            switch (updateMessage.Media?.Length ?? 0)
            {
                // Only when there is no media in the update, and the update's content can fit in one Telegram message
                case 0 when info.FitsInOneTextMessage:
                    return SendSingleTextMessage(info);

                // Only if there is 1 media item, and the update's content can fit as a media caption (in one Telegram message)
                case 1 when info.FitsInOneMediaMessage:
                    return SendSingleMediaMessage(info);

                // Either when:
                // 1. When there is more than 1 media items,
                // 2. When the update's content cannot fit in a single message (text message / single media message)
                default:
                    return SendMessageBatch(info);
            }
        }

        private Task<Message> SendSingleTextMessage(MessageInfo info)
        {
            _logger.LogInformation("Sending text message");

            return _client.SendTextMessageAsync(
                chatId: info.ChatId,
                text: info.Message,
                parseMode: MessageParseMode,
                disableWebPagePreview: DisableWebPagePreview,
                replyToMessageId: info.ReplyMessageId
            );
        }

        private async Task SendMultipleTextMessages(MessageInfo info)
        {
            _logger.LogInformation("Sending multiple text messages");

            string text = info.Message;
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

                int lastMessageId = info.ReplyMessageId;

                foreach (string message in messageChunks)
                {
                    var newInfo = new MessageInfo(
                        message,
                        info.Media,
                        info.ChatId,
                        lastMessageId);
                    
                    Message lastMessage = await SendSingleTextMessage(newInfo);

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

        private Task SendSingleMediaMessage(MessageInfo info)
        {
            return info.Media.Any(media => media.Type == MediaType.Video)
                ? SendVideo(info)
                : SendPhoto(info);
        }

        private Task SendPhoto(MessageInfo info)
        {
            _logger.LogInformation("Sending single photo message");

            var photo = info.Media.FirstOrDefault(media => media.Type == MediaType.Photo);

            return _client.SendPhotoAsync(
                chatId: info.ChatId,
                photo: photo.ToInputOnlineFile(),
                caption: info.Message,
                parseMode: MessageParseMode);
        }

        private Task SendVideo(MessageInfo info)
        {
            _logger.LogInformation("Sending single video message");

            var video = info.Media.FirstOrDefault(media => media.Type == MediaType.Video);

            return _client.SendVideoAsync(
                chatId: info.ChatId,
                video: video.ToInputOnlineFile(),
                caption: info.Message,
                parseMode: MessageParseMode);
        }

        private async Task SendMessageBatch(MessageInfo info)
        {
            // Make sure no other updates will be sent as message batches until this batch is complete sending
            await _messageBatchLock.WaitAsync();

            try
            {
                await SendMessageBatchUnsafe(info);
            }
            finally
            {
                // Release the thread even if the operation fails (avoid a deadlock)
                _messageBatchLock.Release();
            }
        }

        private async Task SendMessageBatchUnsafe(MessageInfo info)
        {
            _logger.LogInformation("Sending message batch");
            
            if (info.FitsInOneMediaMessage)
            {
                await SendMediaAlbumWithCaption(info);
                return;
            }

            int firstMediaMessageId = await SendMediaAlbumIfAny(info);

            var newInfo = new MessageInfo(
                info.Message, info.Media, info.ChatId, firstMediaMessageId);

            await SendTextMessagesIfAny(newInfo);
        }

        private async Task<int> SendMediaAlbumIfAny(MessageInfo info)
        {
            bool hasMedia = info.Media?.Any() ?? false;
            if (!hasMedia)
            {
                return 0;
            }
            
            Message[] mediaMessages = await SendMediaAlbum(info);
            return mediaMessages.FirstOrDefault()?.MessageId ?? 0;
        }

        private async Task SendTextMessagesIfAny(MessageInfo info)
        {
            if (info.Message.Any())
            {
                _logger.LogInformation("Sending corresponding messages");

                if (info.FitsInOneTextMessage)
                {
                    await SendSingleTextMessage(info);
                }
                else
                {
                    await SendMultipleTextMessages(info);
                }
            }
        }

        private Task<Message[]> SendMediaAlbumWithCaption(MessageInfo info)
        {
            IAlbumInputMedia ToAlbumInputMedia(Media media, int index)
            {
                return index > 0
                    ? media.ToAlbumInputMedia()
                    : media.ToAlbumInputMedia(info.Message, MessageParseMode);
            }

            IEnumerable<IAlbumInputMedia> telegramMedia = info.Media
                .Select(ToAlbumInputMedia);

            _logger.LogInformation("Sending media album with caption");

            return _client.SendMediaGroupAsync(telegramMedia, info.ChatId);
        }

        private Task<Message[]> SendMediaAlbum(MessageInfo info)
        {
            var telegramMedia = info.Media
                .Select(media => media.ToAlbumInputMedia());

            _logger.LogInformation("Sending media album");

            return _client.SendMediaGroupAsync(telegramMedia, info.ChatId);
        }
    }
}