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
        
        public string FormattedMessage { get; }

        public Tweet(ITweet tweet)
        {
            Id = tweet.Id;
            Message = tweet.Text;
            Author = new TwitterUser(tweet.CreatedBy);
            CreatedAt = tweet.CreatedAt;
            Url = tweet.Url;

            const string newPostPostedBy = "ציוץ חדש פורסם מאת";
            const string at = "בשעה";
            FormattedMessage =
                $"*{newPostPostedBy}:*" +
                "\n" +
                $"*{Author.DisplayName}* (@{Author.Name})" +
                "\n" +
                $"*{at} {CreatedAt.AddHours(2):HH:mm}:*" +
                "\n \n \n" +
                $"`\"{Message}\"`" +
                "\n \n \n" +
                $"{Url}";
        }
    }
}