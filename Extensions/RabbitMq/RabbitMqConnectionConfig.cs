using System;

namespace Extensions
{
    public class RabbitMqConnectionConfig
    {
        public Uri ConnectionString { get; set; }

        public int ConcurrencyLevel { get; set; } = 1;
    }
}