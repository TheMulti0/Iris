using System;
using Common;

namespace UpdatesProducer
{
    public record LowQualityVideo(
        string Url,
        string RequestUrl,
        string ThumbnailUrl,
        TimeSpan? Duration = null,
        int? Width = null,
        int? Height = null) : Video(Url, ThumbnailUrl, Duration, Width, Height);
}