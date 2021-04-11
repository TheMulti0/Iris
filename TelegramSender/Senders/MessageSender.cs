using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TdLib;
using TelegramClient;

namespace TelegramSender
{
    public class MessageSender
    {
        private readonly ILogger<MessageSender> _logger;
        private readonly ITelegramClient _client;

        public MessageSender(
            ITelegramClient client,
            ILoggerFactory loggerFactory)
        {
            _client = client;
            _logger = loggerFactory.CreateLogger<MessageSender>();
        }

        public async Task<ParsedMessageInfo> ParseAsync(MessageInfo message)
        {
            TdApi.FormattedText text = await ParseTextAsync(message.Message);

            var inputMedia = message.GetInputMessageContentAsync(text);

            var chat = await _client.GetChatAsync(message.ChatId.Identifier);
            
            return new ParsedMessageInfo(
                text,
                inputMedia,
                chat.Id,
                message.ReplyToMessageId,
                message.DisableWebPagePreview,
                message.CancellationToken);
        }

        private Task<TdApi.FormattedText> ParseTextAsync(string text)
        {
            return _client.ParseTextAsync(text, new TdApi.TextParseMode.TextParseModeHTML());
        }

        public async Task<IEnumerable<TdApi.Message>> SendAsync(ParsedMessageInfo parsedMessage)
        {
            if (!parsedMessage.Media.Any())
            {
                return await SendTextMessagesAsync(parsedMessage).ToListAsync();
            }

            IEnumerable<TdApi.Message> mediaMessages = (await SendMediaMessages(parsedMessage))
                .ToList();

            if (parsedMessage.FitsInOneMediaMessage)
            {
                return mediaMessages;
            }

            ParsedMessageInfo inReplyToFirstMediaMessage = parsedMessage with { ReplyToMessageId = mediaMessages.First().Id };
            
            IEnumerable<TdApi.Message> textMessages = await SendTextMessagesAsync(inReplyToFirstMediaMessage)
                .ToListAsync();

            return mediaMessages.Concat(textMessages);
        }

        private async Task<IEnumerable<TdApi.Message>> SendMediaMessages(ParsedMessageInfo parsedMessage)
        {
            List<TdApi.InputMessageContent> audios = parsedMessage.Media
                .Where(content => content is TdApi.InputMessageContent.InputMessageAudio)
                .ToList();
            
            IEnumerable<TdApi.InputMessageContent> nonAudios = parsedMessage.Media
                .Where(content => content is not TdApi.InputMessageContent.InputMessageAudio);

            if (!audios.Any())
            {
                return await SendMessageAlbumAsync(parsedMessage);
            }
            
            IEnumerable<TdApi.Message> audioMessages = await SendMessageAlbumAsync(parsedMessage with { Media = audios });
            IEnumerable<TdApi.Message> nonAudioMessages = await SendMessageAlbumAsync(parsedMessage with { Media = nonAudios });

            return audioMessages.Concat(nonAudioMessages);
        }

        private async Task<IEnumerable<TdApi.Message>> SendMessageAlbumAsync(ParsedMessageInfo parsedMessage)
        {
            return await _client.SendMessageAlbumAsync(
                parsedMessage.ChatId,
                parsedMessage.Media.ToArray(),
                parsedMessage.ReplyToMessageId,
                token: parsedMessage.CancellationToken);
        }

        private async IAsyncEnumerable<TdApi.Message> SendTextMessagesAsync(ParsedMessageInfo message)
        {
            string text = message.Text.Text;
            if (text.Length <= TelegramConstants.MaxTextMessageLength)
            {
                yield return await SendSingleTextMessage(message);
                yield break;
            }
            
            IEnumerable<string> messageChunks = ChunkText(
                text,
                TelegramConstants.MaxTextMessageLength,
                "\n>>>",
                '\n',
                '?',
                '!',
                '.');

            long lastMessageId = 0;
            foreach (string msg in messageChunks)
            {
                TdApi.FormattedText parsedMsg = await ParseTextAsync(msg);
                TdApi.Message textMessage = await SendSingleTextMessage(
                    message with { Text = parsedMsg, ReplyToMessageId = lastMessageId });

                yield return textMessage;
                lastMessageId = textMessage.Id;
            }
        }

        private async Task<TdApi.Message> SendSingleTextMessage(ParsedMessageInfo parsedMessage)
        {
            return await _client.SendMessageAsync(
                parsedMessage.ChatId,
                new TdApi.InputMessageContent.InputMessageText
                {
                    Text = parsedMessage.Text,
                    DisableWebPagePreview = parsedMessage.DisableWebPagePreview
                },
                parsedMessage.ReplyToMessageId,
                token: parsedMessage.CancellationToken);
        }

        private static IEnumerable<string> ChunkText(
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
    }
}