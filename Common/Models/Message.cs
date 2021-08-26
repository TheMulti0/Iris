using System.Collections.Generic;
using Scraper.RabbitMq.Common;

namespace Common
{
    public record Message(
        NewPost NewPost,
        List<UserChatSubscription> DestinationChats)
    {
        public override string ToString()
        {
            return $"{NewPost.Post.Url}, destined to {DestinationChats.Count} chat/s";
        }
    }
}