using System.Collections.Generic;

namespace Common
{
    public record Message(
        Update Update,
        List<UserChatSubscription> DestinationChats)
    {
        public override string ToString()
        {
            return $"Message: {Update}, Destined to {DestinationChats.Count} chats";
        }
    }
}