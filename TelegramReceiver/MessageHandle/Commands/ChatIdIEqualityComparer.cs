using System.Collections.Generic;
using Telegram.Bot.Types;

namespace TelegramReceiver
{
    internal class ChatIdIEqualityComparer : IEqualityComparer<ChatId>
    {
        public bool Equals(ChatId? x, ChatId? y)
        {
            return x.GetHashCode() == y.GetHashCode();
        }

        public int GetHashCode(ChatId obj)
        {
            return obj.GetHashCode();
        }
    }
}