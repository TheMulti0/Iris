using System;
using System.Text.Json.Serialization;

namespace Common
{
    public record Video : IMedia
    {
        public string Url { get; init; }
        public string ThumbnailUrl { get; init; }
        [JsonConverter(typeof(NullableTimeSpanConverter))]
        public TimeSpan? Duration { get; init; }
        public int? Width { get; init; }
        public int? Height { get; init; }

        public Video(
            string url,
            string thumbnailUrl,
            TimeSpan? duration = null,
            int? width = null,
            int? height = null)
        {
            Url = url;
            ThumbnailUrl = thumbnailUrl;
            Duration = duration;
            Width = width;
            Height = height;
        }
    }
}