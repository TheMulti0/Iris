using System;

namespace FacebookScraper
{
    public record SharedPost
    {
        public string Id { get; init; }

        public string Url { get; init; }

        public string Text { get; init; }

        public DateTime? CreationDate { get; init; }

        public Author Author { get; init; }
    }
}