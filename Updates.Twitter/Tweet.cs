using System;
using Tweetinvi.Models;
using Updates.Api;
using IUser = Updates.Api.IUser;

namespace Updates.Twitter
{
    public class Tweet : IUpdate
    {
        public long Id { get; }
        
        public string Message { get; }
        
        public IUser Author { get; }
        
        public DateTime CreatedAt { get; }
        
        public string Url { get; }

        public Tweet(ITweet tweet)
        {
            Id = tweet.Id;
            Message = tweet.Text;
            Author = new TwitterUser(tweet.CreatedBy);
            CreatedAt = tweet.CreatedAt;
            Url = tweet.Url;
        }

        public bool Equals(IUpdate other) => Id == other?.Id;
    }
}