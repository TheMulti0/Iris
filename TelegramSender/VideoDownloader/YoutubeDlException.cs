using System;

namespace TelegramSender
{
    public class YoutubeDlException : Exception
    {
        public YoutubeDlException(string message) : base(message)
        {
        }
    }
}