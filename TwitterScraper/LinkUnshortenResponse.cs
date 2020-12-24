namespace TwitterScraper
{
    public sealed record LinkUnshortenResponse(
        string Title,
        string Description,
        string RedirectUrl,
        string ShortUrl,
        string UrlHash,
        bool GoogleSafeBrowsing,
        bool BadDomain);
}