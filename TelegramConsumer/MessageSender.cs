using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Remutable.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using static TelegramConsumer.TelegramConstants;

namespace TelegramConsumer
{
    public class MessageSender
    {
        private readonly ITelegramBotClient _client;
        private readonly ILogger<MessageSender> _logger;
        private readonly SemaphoreSlim _messageBatchLock = new SemaphoreSlim(1, 1);
        private readonly TextSender _textSender;

        public MessageSender(
            ITelegramBotClient client,
            ILoggerFactory loggerFactory)
        {
            _client = client;
            _logger = loggerFactory.CreateLogger<MessageSender>();
            _textSender = new TextSender(
                client,
                loggerFactory.CreateLogger<TextSender>());
        }

        public Task SendAsync(MessageInfo message)
        {
            _logger.LogWarning(
                "Message length: {}",
                message.Message.Length);

            switch (message.Media?.Length ?? 0)
            {
                // Only when there is no media in the update, and the update's content can fit in one Telegram message
                case 0:
                    return _textSender.SendAsync(message);

                // Only if there is 1 media item, and the update's content can fit as a media caption (in one Telegram message)
                case 1 when message.FitsInOneMediaMessage:
                    return SendSingleMediaMessage(message);

                // Either when:
                // 1. When there is more than 1 media items,
                // 2. When the update's content cannot fit in a single message (text message / single media message)
                default:
                    return SendMessageBatch(message);
            }
        }

        private Task<Message> SendSingleTextMessage(MessageInfo message)
        {
            _logger.LogInformation("Sending text message");

            return _client.SendTextMessageAsync(
                chatId: message.ChatId,
                text: message.Message,
                parseMode: MessageParseMode,
                disableWebPagePreview: DisableWebPagePreview,
                replyToMessageId: message.ReplyMessageId,
                cancellationToken: message.CancellationToken
            );
        }

        private async Task SendMultipleTextMessages(MessageInfo message)
        {
            _logger.LogInformation("Sending multiple text messages");

            string text = message.Message;
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

                int lastMessageId = message.ReplyMessageId;

                foreach (string msg in messageChunks)
                {
                    MessageInfo newInfo = message
                        .Remute(i => i.Message, msg)
                        .Remute(i => i.ReplyMessageId, lastMessageId);
                    
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

        private Task SendSingleMediaMessage(MessageInfo message)
        {
            return message.Media.Any(media => media.Type == MediaType.Video)
                ? SendVideo(message)
                : SendPhoto(message);
        }

        private Task SendPhoto(MessageInfo message)
        {
            _logger.LogInformation("Sending single photo message");

            var photo = message.Media.FirstOrDefault(media => media.Type == MediaType.Photo);

            return _client.SendPhotoAsync(
                chatId: message.ChatId,
                photo: photo.ToInputOnlineFile(),
                caption: message.Message,
                parseMode: MessageParseMode,
                cancellationToken: message.CancellationToken);
        }

        private Task SendVideo(MessageInfo message)
        {
            _logger.LogInformation("Sending single video message");

            var video = message.Media.FirstOrDefault(media => media.Type == MediaType.Video);

            return _client.SendVideoAsync(
                chatId: message.ChatId,
                video: video.ToInputOnlineFile(),
                caption: message.Message,
                parseMode: MessageParseMode,
                cancellationToken: message.CancellationToken);
        }

        private async Task SendMessageBatch(MessageInfo message)
        {
            // Make sure no other updates will be sent as message batches until this batch is complete sending
            await _messageBatchLock.WaitAsync();

            try
            {
                await SendMessageBatchUnsafe(message);
            }
            finally
            {
                // Release the thread even if the operation fails (avoid a deadlock)
                _messageBatchLock.Release();
            }
        }

        private async Task SendMessageBatchUnsafe(MessageInfo message)
        {
            _logger.LogInformation("Sending message batch");
            
            if (message.FitsInOneMediaMessage)
            {
                await SendMediaAlbumWithCaption(message);
                return;
            }

            int firstMediaMessageId = await SendMediaAlbumIfAny(message);

            MessageInfo newInfo = message.Remute(i => i.ReplyMessageId, firstMediaMessageId);

            if (newInfo.Message.Any())
            {
                await _textSender.SendAsync(newInfo);
            }
        }

        private async Task<int> SendMediaAlbumIfAny(MessageInfo message)
        {
            bool hasMedia = message.Media?.Any() ?? false;
            if (!hasMedia)
            {
                return 0;
            }
            
            Message[] mediaMessages = await SendMediaAlbum(message);
            return mediaMessages.FirstOrDefault()?.MessageId ?? 0;
        }

        private async Task SendTextMessagesIfAny(MessageInfo message)
        {
            if (message.Message.Any())
            {
                _logger.LogInformation("Sending corresponding messages");

                if (message.FitsInOneTextMessage)
                {
                    await SendSingleTextMessage(message);
                }
                else
                {
                    await SendMultipleTextMessages(message);
                }
            }
        }

        private Task<Message[]> SendMediaAlbumWithCaption(MessageInfo message)
        {
            IAlbumInputMedia ToAlbumInputMedia(Media media, int index)
            {
                return index > 0
                    ? media.ToAlbumInputMedia()
                    : media.ToAlbumInputMedia(message.Message, MessageParseMode);
            }

            IEnumerable<IAlbumInputMedia> telegramMedia = message.Media
                .Select(ToAlbumInputMedia);

            _logger.LogInformation("Sending media album with caption");

            return _client.SendMediaGroupAsync(
                inputMedia: telegramMedia,
                chatId: message.ChatId, 
                cancellationToken: message.CancellationToken);
        }

        private Task<Message[]> SendMediaAlbum(MessageInfo message)
        {
            var telegramMedia = message.Media
                .Select(media => media.ToAlbumInputMedia());

            _logger.LogInformation("Sending media album");

            return _client.SendMediaGroupAsync(
                inputMedia: telegramMedia,
                chatId: message.ChatId,
                cancellationToken: message.CancellationToken);
        }
    }
}