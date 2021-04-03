﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using TdLib;

namespace TelegramClient
{
    public class MessageSendFailedException : Exception
    {
        public MessageSendFailedException(string message) : base(message)
        {
        }
    }
    
    public class TelegramClient : ITelegramClient
    {
        private readonly TdClient _client;
        public IObservable<TdApi.Update> OnUpdateReceived { get; }

        public TelegramClient(TdClient client)
        {
            _client = client;
            OnUpdateReceived = client.OnUpdateReceived();
        }

        public Task<TdApi.Chat> GetChatAsync(long chatId)
        {
            return _client.GetChatAsync(chatId);
        }

        public async Task<TdApi.Message> SendMessageAsync(
            long chatId,
            TdApi.InputMessageContent inputMessageContent,
            long replyToMessageId = 0,
            TdApi.ReplyMarkup replyMarkup = null,
            TdApi.SendMessageOptions options = null)
        {
            TdApi.Message message = await _client.SendMessageAsync(
                chatId: chatId,
                inputMessageContent: inputMessageContent,
                replyToMessageId: replyToMessageId,
                replyMarkup: replyMarkup,
                options: options);
            
            return await OnUpdateReceived
                .Where(u =>
                {
                    switch (u)
                    {
                        case TdApi.Update.UpdateMessageSendSucceeded m:
                            return m.OldMessageId == message.Id;
                        
                        case TdApi.Update.UpdateMessageSendFailed f when f.OldMessageId == message.Id:
                            throw new MessageSendFailedException(f.ErrorMessage);
                        
                        default:
                            return false;
                    }
                })
                .Cast<TdApi.Update.UpdateMessageSendSucceeded>()
                .Select(u => u.Message)
                .FirstOrDefaultAsync();
        }
        
        public async Task<IEnumerable<TdApi.Message>> SendMessageAlbumAsync(
            long chatId,
            TdApi.InputMessageContent[] inputMessageContents,
            long replyToMessageId = 0,
            TdApi.SendMessageOptions options = null)
        {
            TdApi.Messages messages = await _client.SendMessageAlbumAsync(
                chatId: chatId,
                inputMessageContents: inputMessageContents,
                replyToMessageId: replyToMessageId,
                options: options);

            return await GetSentMessages(messages)
                .ToListAsync();
        }

        private async IAsyncEnumerable<TdApi.Message> GetSentMessages(TdApi.Messages messages)
        {
            var sentMessagesCount = 0;

            await foreach (TdApi.Update update in OnUpdateReceived.ToAsyncEnumerable())
            {
                switch (update)
                {
                    case TdApi.Update.UpdateMessageSendFailed f:
                        throw new MessageSendFailedException(f.ErrorMessage);
                    
                    case TdApi.Update.UpdateMessageSendSucceeded m when messages.Messages_.All(message => message.Id != m.OldMessageId):
                        continue;
                    
                    case TdApi.Update.UpdateMessageSendSucceeded m:
                    {
                        yield return m.Message;

                        sentMessagesCount++;

                        if (messages.TotalCount == sentMessagesCount)
                        {
                            yield break;
                        }
                        break;
                    }
                }
            }
        }
    }
}