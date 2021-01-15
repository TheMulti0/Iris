using UpdatesScraper;

namespace TwitterScraper
{
    public class TwitterUpdatesProviderConfig : UpdatesProviderBaseConfig
    {
        public string ConsumerKey { get; set; }

        public string ConsumerSecret { get; set; }
    }
}