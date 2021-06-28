using System;
using System.Collections.Generic;

namespace FacebookScraper
{
    public record Post
    {
        public string Id { get; init; }

        public string EntireText { get; init; }
        
        public string PostTextOnly { get; init; }
        
        public DateTime? CreationDate { get; init; }
        
        public string Url { get; init; }
        
        public Author Author { get; init; }

        public bool IsLive { get; init; }
        
        public string Link { get; init; }

        public IEnumerable<Image> Images { get; init; }

        public Video Video { get; init; }

        public Stats Stats { get; init; }

        public SharedPost SharedPost { get; init; }

        public IEnumerable<RootComment> Comments { get; init; }
    }
}