using System;

namespace UpdatesScraper
{
    public record VideoInfo(
        string Url,
        string ThumbnailUrl,
        TimeSpan? Duration,
        int? Width,
        int? Height);
}