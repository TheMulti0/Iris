using UpdatesScraper;

namespace TwitterScraper
{
    public class TwitterUpdatesProviderConfig : UpdatesProviderBaseConfig
    {
        public string ConsumerKey { get; set; }

        public string ConsumerSecret { get; set; }
        
        public string AccessToken { get; set; }
        
        public string AccessTokenSecret { get; set; }
    }
}