namespace FacebookScraper
{
    public class FacebookUpdatesProviderConfig
    {
        public int PageCount { get; set; } = 1;

        public string[] Proxies { get; set; } = new string[0];

        public string CookiesFileName { get; set; }
    }
}