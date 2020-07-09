using Extensions;

namespace ConfigProducer
{
    public class KafkaConfig : BaseKafkaConfig
    {
        public string ConfigsTopic { get; set; }
    }
}