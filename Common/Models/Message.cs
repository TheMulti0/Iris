﻿using System.Collections.Generic;
using Scraper.RabbitMq.Common;

namespace Common
{
    public record Message(
        NewPost NewPost,
        List<UserChatSubscription> DestinationChats)
    {
        public override string ToString()
        {
            return $"NewPost: {NewPost}, Destined to {DestinationChats.Count} chats";
        }
    }
}