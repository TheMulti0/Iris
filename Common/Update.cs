using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Common
{
    public record Update
    {
        public string Content { get; init; }

        public string AuthorId { get; init; }

        public DateTime? CreationDate { get; init; }

        public string Url { get; init; }

        public List<IMedia> Media { get; init; }

        public bool Repost { get; init; }
        
        public string Source { get; init; }

        public override string ToString()
        {
            return $"{Url} (By {AuthorId}, Media length: {Media?.Count}, Repost: {Repost})";
        }
    }
}