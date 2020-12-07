﻿namespace Extensions
{
    public class BaseKafkaConfig
    {
        public string BrokersServers { get; set; }

        public string Topic { get; set; }

        public SerializationType KeySerializationType { get; set; }
        
        public SerializationType ValueSerializationType { get; set; }
    }
}