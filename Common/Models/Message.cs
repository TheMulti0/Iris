using System.Collections.Generic;

namespace Common
{
    public record Message(
        Update Update,
        List<UserChatSubscription> DestinationChats);
}