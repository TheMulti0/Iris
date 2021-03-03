using System;
using System.Collections.Generic;

namespace Common
{
    public record Update
    {
        public string Content { get; init; }

        public User Author { get; init; }

        public DateTime? CreationDate { get; init; }

        public string Url { get; init; }

        public List<IMedia> Media { get; init; }

        public bool IsRepost { get; init; }

        public bool IsLive { get; set; }

        public bool IsReply { get; set; }
        
        public byte[] Screenshot { get; set; }


        public override string ToString()
        {
            return $"Update: {Url} (by {Author})";
        }
    }
}