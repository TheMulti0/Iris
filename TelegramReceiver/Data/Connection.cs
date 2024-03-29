﻿using Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using User = Telegram.Bot.Types.User;

namespace TelegramReceiver
{
    public class Connection : IConnectionProperties
    {
        [BsonId]
        public ObjectId _id { get; set; }
        
        public User User { get; set; }

        public Language Language { get; set; }
        public long ChatId { get; set; }

        public bool HasAgreedToTos { get; set; }

        public Connection()
        {
        }
        
        public Connection(IConnectionProperties properties)
        {
            Language = properties.Language;
            ChatId = properties.ChatId;
            HasAgreedToTos = properties.HasAgreedToTos;
        }
    }
}