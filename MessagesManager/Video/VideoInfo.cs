namespace MessagesManager
{
    internal record VideoInfo
    {
        public string ExtractedUrl { get; init; }
        public string ThumbnailUrl { get; init; }
        public double? DurationSeconds { get; init; }
        public int? Width { get; init; }
        public int? Height { get; init; }
    }
}