using System.Collections.Generic;
using Scraper.MassTransit.Common;

namespace Common
{
    public record SendMessage(
        NewPost NewPost,
        List<UserChatSubscription> DestinationChats)
    {
        public override string ToString()
        {
            return $"{NewPost.Post.Url}, destined to {DestinationChats.Count} chat/s";
        }
    }
}