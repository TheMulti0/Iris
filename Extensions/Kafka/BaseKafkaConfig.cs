namespace Extensions
{
    public class BaseKafkaConfig
    {
        public string BrokersServers { get; set; }

        public virtual string DefaultTopic { get; set; }

        public SerializationType KeySerializationType { get; set; }
        
        public SerializationType ValueSerializationType { get; set; }
    }
}