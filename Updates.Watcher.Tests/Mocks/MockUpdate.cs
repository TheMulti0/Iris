using System;
using Updates.Api;

namespace Updates.Watcher.Tests
{
    internal class MockUpdate : IUpdate
    {
        public long Id { get; }
        
        public string Message { get; }
        
        public IUser Author { get; }
        
        public DateTime CreatedAt { get; }
        
        public string Url { get; }
        
        public string FormattedMessage => Url;

        public MockUpdate(
            long id,
            string message,
            IUser author,
            DateTime createdAt,
            string url)
        {
            Id = id;
            Message = message;
            Author = author;
            CreatedAt = createdAt;
            Url = url;
        }
    }
}