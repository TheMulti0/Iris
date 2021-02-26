using System.Collections.Generic;
using Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace SubscriptionsDataLayer
{
    [CollectionName("subscriptions")]
    public class SubscriptionEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }
        
        public User User { get; set; }

        public int Version { get; set; }

        public List<UserChatSubscription> Chats { get; set; } = new();
    }
}