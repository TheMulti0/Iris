using System.Collections.Generic;
using Common;

namespace SubscriptionsManager
{
    internal class UserChatSubscriptionEqualityComparer : IEqualityComparer<UserChatSubscription>
    {
        public bool Equals(UserChatSubscription x, UserChatSubscription y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }
            if (ReferenceEquals(x, null))
            {
                return false;
            }
            if (ReferenceEquals(y, null))
            {
                return false;
            }
            if (x.GetType() != y.GetType())
            {
                return false;
            }
            return x.ChatId == y.ChatId;
        }

        public int GetHashCode(UserChatSubscription obj)
        {
            return obj.ChatId != null ? obj.ChatId.GetHashCode() : 0;
        }
    }
}