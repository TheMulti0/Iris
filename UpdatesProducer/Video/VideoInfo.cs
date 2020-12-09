using System;

namespace UpdatesProducer
{
    public record VideoInfo(
        string Url,
        string ThumbnailUrl,
        TimeSpan? Duration,
        int? Width,
        int? Height);
}