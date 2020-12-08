namespace Common
{
    public class KafkaConfig
    {
        public string BrokersServers { get; set; }
        
        public TopicConfig Updates { get; set; }
        
        public TopicConfig Configs { get; set; }
    }
}