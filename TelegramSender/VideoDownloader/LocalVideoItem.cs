using System;
using FFMpegCore;
using Scraper.Net;

namespace TelegramSender
{
    public record LocalVideoItem : IMediaItem
    {
        public string Url { get; }

        public string ThumbnailUrl { get; init; }

        public bool IsThumbnailLocal { get; init; }

        public TimeSpan Duration { get; }
        
        public int? Width { get; }
        
        public int? Height { get; }

        public LocalVideoItem(
            string path,
            string thumbnailUrl,
            IMediaAnalysis analysis)
        {
            Url = path;
            ThumbnailUrl = thumbnailUrl;
            IsThumbnailLocal = thumbnailUrl != null;
            Duration = analysis.Duration;
            Width = analysis.PrimaryVideoStream?.Width;
            Height = analysis.PrimaryVideoStream?.Height;
        }
    }
}