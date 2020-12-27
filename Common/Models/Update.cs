using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Common
{
    public record Update
    {
        public string Content { get; init; }

        public User Author { get; init; }

        public DateTime? CreationDate { get; init; }

        public string Url { get; init; }

        public List<IMedia> Media { get; init; }

        public bool Repost { get; init; }
        
        public string Source { get; init; }

        public override string ToString()
        {
            return $"{Url} (By {Author}, Media length: {Media?.Count}, Repost: {Repost})";
        }
    }
}