﻿namespace FacebookScraper
{
    public record Author
    {
        public string Id { get; init; }

        public string Url { get; init; }

        public string UserName { get; init; }
    }
}