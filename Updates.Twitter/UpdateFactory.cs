using System;
using Tweetinvi.Models;
using Updates.Api;

namespace Updates.Twitter
{
    public static class UpdateFactory
    {
        public static Update ToUpdate(ITweet tweet)
        {
            long id = tweet.Id;
            string message = tweet.Text;
            var author = UserFactory.ToUser(tweet.CreatedBy);
            DateTime createdAt = tweet.CreatedAt;
            string url = tweet.Url;

            const string newPostPostedBy = "ציוץ חדש פורסם כעט מאת";
            string formattedMessage =
                $"*{newPostPostedBy}:*" +
                "\n" +
                $"*{author.DisplayName}* (@{author.Name})" +
                "\n \n \n" +
                $"`\"{message}\"`" +
                "\n \n \n" +
                $"{url}";
            
            return new Update(
                id,
                message,
                author,
                createdAt,
                url,
                formattedMessage);
        }
    }
}