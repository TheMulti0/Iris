using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Logging;
using TdLib;
using Telegram.Bot;
using TelegramClient;

namespace TelegramSender
{
    public class MessageSender
    {
        private readonly ILogger<MessageSender> _logger;
        private readonly TextSender _textSender;
        private readonly AudioSender _audioSender;
        private readonly MediaSender _mediaSender;
        private readonly ITelegramClient _client;

        public MessageSender(
            ITelegramBotClient client,
            ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<MessageSender>();

            _textSender = new TextSender(
                client,
                loggerFactory.CreateLogger<TextSender>());

            _audioSender = new AudioSender(
                client,
                loggerFactory.CreateLogger<AudioSender>());
            
            _mediaSender = new MediaSender(
                client,
                _textSender);
        }

        public Task SendAsync(MessageInfo message)
        {
            if (message.Media.Any(media => media is Audio))
            {
                return _audioSender.SendAsync(
                    message,
                    (Audio) message.Media.FirstOrDefault(media => media is Audio));
            }
            
            return message.Media.Count(media => media is not Audio) switch 
            {
                0 => _textSender.SendAsync(message),
                _ => _mediaSender.SendAsync(message)
            };
        }

        public async Task<ParsedMessageInfo> ParseAsync(MessageInfo message)
        {
            TdApi.FormattedText text = await _client.ParseTextAsync(message.Message, new TdApi.TextParseMode.TextParseModeHTML());

            var inputMedia = message.GetInputMedia(text);
            
            return new ParsedMessageInfo(
                text,
                inputMedia,
                message.ChatId.Identifier,
                message.ReplyToMessageId,
                message.DisableWebPagePreview,
                message.CancellationToken);
        }

        public async Task<IEnumerable<TdApi.Message>> SendAsync(ParsedMessageInfo parsedMessage)
        {
            if (!parsedMessage.Media.Any())
            {
                return await SendTextMessagesAsync(parsedMessage);
            }
            
            List<TdApi.Message> mediaMessages = (await SendMessageAlbumAsync(parsedMessage)).ToList();

            if (parsedMessage.FitsInOneMediaMessage)
            {
                return mediaMessages;
            }
            
            IEnumerable<TdApi.Message> textMessages = await SendTextMessagesAsync(parsedMessage);

            return mediaMessages.Concat(textMessages);
        }

        private async Task<IEnumerable<TdApi.Message>> SendMessageAlbumAsync(ParsedMessageInfo parsedMessage)
        {
            return await _client.SendMessageAlbumAsync(
                parsedMessage.ChatId,
                parsedMessage.Media.ToArray(),
                parsedMessage.ReplyToMessageId,
                token: parsedMessage.CancellationToken);
        }

        private async Task<IEnumerable<TdApi.Message>> SendTextMessagesAsync(ParsedMessageInfo parsedMessage)
        {
            // todo Trunctuate
            return new[]
            {
                await _client.SendMessageAsync(
                    parsedMessage.ChatId,
                    new TdApi.InputMessageContent.InputMessageText
                    {
                        Text = parsedMessage.Text
                    },
                    parsedMessage.ReplyToMessageId,
                    token: parsedMessage.CancellationToken)  
            };
        }
    }
}