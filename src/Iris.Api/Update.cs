using System;
using System.Collections.Generic;

namespace Iris.Api
{
    public class Update
    {
        public long Id { get; }

        public User Author { get; }

        public string Message { get; }

        public string FormattedMessage { get; }

        public DateTime CreatedAt { get; }

        public string Url { get; }

        public IEnumerable<Media> Media { get; }
        
        public Update(
            long id,
            User author,
            string message,
            string formattedMessage,
            DateTime createdAt,
            string url,
            IEnumerable<Media> media)
        {
            Id = id;
            Author = author;
            Message = message;
            FormattedMessage = formattedMessage;
            CreatedAt = createdAt;
            Url = url;
            Media = media;
        }
    }
}