using System;

namespace Common
{
    public record Video(
        string Url,
        string ThumbnailUrl,
        TimeSpan? Duration = null,
        int? Width = null,
        int? Height = null) : IMedia;
}