using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Remutable.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramConsumer
{
    public class TextSender
    {
        private readonly ITelegramBotClient _client;
        private readonly ILogger<TextSender> _logger;

        public TextSender(
            ITelegramBotClient client,
            ILogger<TextSender> logger)
        {
            _client = client;
            _logger = logger;
        }

        public Task SendAsync(MessageInfo message)
        {
            return message.FitsInOneTextMessage 
                ? SendSingleTextMessage(message) 
                : SendMultipleTextMessages(message);
        }

        private Task<Message> SendSingleTextMessage(MessageInfo message)
        {
            _logger.LogInformation("Sending single text message");

            return _client.SendTextMessageAsync(
                chatId: message.ChatId,
                text: message.Message,
                parseMode: TelegramConstants.MessageParseMode,
                disableWebPagePreview: TelegramConstants.DisableWebPagePreview,
                replyToMessageId: message.ReplyMessageId,
                cancellationToken: message.CancellationToken
                );
        }

        private async Task SendMultipleTextMessages(MessageInfo message)
        {
            _logger.LogInformation("Sending multiple text messages");

            string text = message.Message;
            int textLength = text.Length;
            if (textLength > TelegramConstants.MaxTextMessageLength)
            {
                IEnumerable<string> messageChunks = ChunkifyText(
                    text,
                    TelegramConstants.MaxTextMessageLength,
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
    }
}