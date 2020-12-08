using System;

namespace Common
{
    public record Audio(
        string Url,
        string ThumbnailUrl,
        TimeSpan? Duration,
        string Title,
        string Artist) : IMedia;
}
