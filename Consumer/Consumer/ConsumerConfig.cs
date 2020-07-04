namespace Consumer
{
    public class ConsumerConfig
    {
        public string[] Topics { get; set; }

        public double PollIntervalSeconds { get; set; }

        public string GroupId { get; set; }

        public string BrokersServers { get; set; }
    }
}