using System;

namespace FacebookScraper
{
    public record Image
    {
        public string Id { get; init; }
        
        public string Url { get; init; }

        public string LowQualityUrl { get; init; }
        
        public string Description { get; init; }
    }
    
    public record SharedPost
    {
        public string Id { get; init; }

        public string Url { get; init; }

        public string Text { get; init; }

        public DateTime? CreationDate { get; init; }

        public Author Author { get; init; }
    }
    
    public record Stats
    {
        public int Comments { get; init; }

        public int Shares { get; init; }

        public int Likes { get; init; }
    }
    
    public record Author
    {
        public string Id { get; init; }

        public string Url { get; init; }

        public string UserName { get; init; }
    }
    
    public record Video
    {
        public string Id { get; init; }

        public string Url { get; init; }

        public TimeSpan? Duration { get; init; }

        public int? Width { get; init; }

        public int? Height { get; init; }

        public string Quality { get; init; }

        public string ThumbnailUrl { get; init; }

        public double? SizeMb { get; init; }

        public int? Watches { get; init; }
    }
}