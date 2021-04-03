using System;

namespace TelegramClient
{
    public class MessageSendFailedException : Exception
    {
        public MessageSendFailedException(string message) : base(message)
        {
        }
    }
}