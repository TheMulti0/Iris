namespace Consumer
{
    public class TopicConsumerConfig
    {
        public string Topic { get; set; }

        public double PollIntervalSeconds { get; set; }

        public string GroupId { get; set; }

        public string BootstrapServers { get; set; }
    }
}