using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Telegram.Bot.Types;

namespace TelegramReceiver.Data
{
    internal class Connection
    {
        [BsonId]
        public ObjectId? _id { get; set; }
        
        public User User { get; set; }

        public string Chat { get; set; }
    }
}