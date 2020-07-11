namespace Extensions
{
    public class ConsumerConfig : BaseKafkaConfig
    {
        public string[] SubscriptionTopics { get; set; }

        public override string DefaultTopic => SubscriptionTopics[0];

        public string GroupId { get; set; }
    }
}