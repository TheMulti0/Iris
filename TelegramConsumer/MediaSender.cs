using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Remutable.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramConsumer
{
    public class MediaSender
    {
        private readonly ITelegramBotClient _client;
        private readonly TextSender _textSender;
        private readonly ILogger<MediaSender> _logger;
        private readonly SemaphoreSlim _messageBatchLock = new SemaphoreSlim(1, 1);

        public MediaSender(
            ITelegramBotClient client,
            TextSender textSender,
            ILogger<MediaSender> logger)
        {
            _client = client;
            _textSender = textSender;
            _logger = logger;
        }

        public async Task SendAsync(MessageInfo message)
        {
            // Make sure no other updates will be sent as message batches until this batch is complete sending
            await _messageBatchLock.WaitAsync();

            try
            {
                Task sendTask = message.FitsInOneMediaMessage 
                    ? SendMediaAlbumWithCaption(message) 
                    : SendMediaAlbumWithAdditionalTextMessage(message);
                
                sendTask.Wait();
            }
            finally
            {
                // Release the thread even if the operation fails (avoid a deadlock)
                _messageBatchLock.Release();
            }
        }

        private Task<Message[]> SendMediaAlbumWithCaption(MessageInfo message)
        {
            IAlbumInputMedia ToAlbumInputMedia(Media media, int index)
            {
                return index > 0
                    ? media.ToAlbumInputMedia()
                    : media.ToAlbumInputMedia(message.Message, TelegramConstants.MessageParseMode);
            }

            _logger.LogInformation("Sending media album with caption");

            IEnumerable<IAlbumInputMedia> telegramMedia = message.Media
                .Select(ToAlbumInputMedia);

            return _client.SendMediaGroupAsync(
                inputMedia: telegramMedia,
                chatId: message.ChatId, 
                cancellationToken: message.CancellationToken);
        }

        private async Task SendMediaAlbumWithAdditionalTextMessage(MessageInfo message)
        {
            _logger.LogInformation("Sending media album with additional text message");
            
            var firstMediaMessageId = 0;
            
            if (message.Media.Any())
            {
                firstMediaMessageId = await SendMediaAlbumIfAny(message);
            }

            if (message.Message.Any())
            {
                MessageInfo newMessage = message
                    .Remute(i => i.ReplyMessageId, firstMediaMessageId);

                await _textSender.SendAsync(newMessage);
            }
        }

        private async Task<int> SendMediaAlbumIfAny(MessageInfo message)
        {
            _logger.LogInformation("Sending media album");

            var telegramMedia = message.Media
                .Select(media => media.ToAlbumInputMedia());
            
            Message[] mediaMessages = await _client.SendMediaGroupAsync(
                inputMedia: telegramMedia,
                chatId: message.ChatId,
                cancellationToken: message.CancellationToken);
            
            return mediaMessages.FirstOrDefault()?.MessageId ?? 0;
        }
    }
}