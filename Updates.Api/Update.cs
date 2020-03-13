using System;

namespace Updates.Api
{
    public class Update
    {
        public long Id { get; }

        public string Message { get; }

        public User Author { get; }

        public DateTime CreatedAt { get; }

        public string Url { get; }
        
        public string FormattedMessage { get; }
        
        public Update(
            long id,
            string message,
            User author,
            DateTime createdAt,
            string url,
            string formattedMessage)
        {
            Id = id;
            Message = message;
            Author = author;
            CreatedAt = createdAt;
            Url = url;
            FormattedMessage = formattedMessage;
        }
    }
}