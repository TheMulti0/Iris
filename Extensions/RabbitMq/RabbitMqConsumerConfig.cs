namespace Extensions
{
    public class RabbitMqConsumerConfig
    {
        public string Queue { get; set; }

        public bool AckOnlyOnSuccess { get; set; }
    }
}