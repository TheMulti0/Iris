using System;

namespace Extensions
{
    public class RabbitMqConfig
    {
        public Uri ConnectionString { get; set; }

        public string Destination { get; set; }
    }
}