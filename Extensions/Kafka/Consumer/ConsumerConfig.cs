namespace Extensions
{
    public class ConsumerConfig : BaseKafkaConfig
    {
        public string[] Topics { get; set; }

        public string GroupId { get; set; }
    }
}