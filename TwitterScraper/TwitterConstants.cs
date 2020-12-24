namespace TwitterScraper
{
    public static class TwitterConstants
    {
        public const string TwitterBaseDomain = "twitter.com";

        public static readonly string TwitterBaseUrl = $"https://{TwitterBaseDomain}";
        public static readonly string TwitterBaseUrlWww = $"https://www.{TwitterBaseDomain}";

        public const string LinkunshortenBaseUrl = "https://linkunshorten.com/api";
        
        public const string FacebookIncorrectRedirectUrl = "https://www.facebook.com/unsupportedbrowser";
    }
}