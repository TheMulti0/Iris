using System;

namespace Common
{
    public record Video(
        string Url,
        string ThumbnailUrl,
        bool IsBestFormat,
        // TODO consider moving field
        TimeSpan? Duration = null,
        int? Width = null,
        int? Height = null) : IMedia;
}