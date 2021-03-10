using System;

namespace UpdatesDb
{
    public class MongoDbConfig
    {
        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }

        public TimeSpan? UpdatesExpiration { get; set; }
    }
}