using Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using User = Telegram.Bot.Types.User;

namespace TelegramReceiver
{
    public class Connection
    {
        [BsonId]
        public ObjectId _id { get; set; }
        
        public User User { get; set; }

        public string Chat { get; set; }

        public Language Language { get; set; }
    }
}