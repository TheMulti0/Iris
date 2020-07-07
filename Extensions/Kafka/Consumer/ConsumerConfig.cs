namespace Extensions
{
    public class ConsumerConfig
    {
        public string[] Topics { get; set; }

        public string GroupId { get; set; }

        public string BrokersServers { get; set; }
    }
}