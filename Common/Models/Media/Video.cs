using System;
using System.Text.Json.Serialization;

namespace Common
{
    public record Video
    {
        public string Url { get; }
        public string ThumbnailUrl { get; init; }
        [JsonConverter(typeof(NullableTimeSpanConverter))]
        public TimeSpan? Duration { get; }
        public int? Width { get; }
        public int? Height { get; }

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