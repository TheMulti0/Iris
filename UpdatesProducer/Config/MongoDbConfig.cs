using System;

namespace UpdatesProducer
{
    public class MongoDbConfig
    {
        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }

        public TimeSpan? SentUpdatesExpiration { get; set; }
    }
}