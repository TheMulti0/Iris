using System;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Primitives;
using Tweetinvi.Logic;
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

            var formattedMessage = new StringBuilder("ציוץ חדש פורסם כעת מאת: \n")
                .Append(GetTweetText(tweet));

            if (tweet.QuotedTweet != null)
            {
                formattedMessage.Append(
                    "\n הציוץ הזה הוא בתגובה לציוץ הבא מאת: \n" +
                    GetTweetText(tweet.QuotedTweet));
            }
            
            formattedMessage.Append(
                "\n \n \n \n" +
                $"{url}");

            return new Update(
                id,
                message,
                author,
                createdAt,
                url,
                formattedMessage.ToString());
        }

        private static string GetTweetText(ITweet tweet)
        {
            IUser author = tweet.CreatedBy;
            
            return $"*{author.Name}* (@{author.ScreenName})" +
                   "\n \n \n" +
                   $"`\"{tweet.Text}\"`";
        }
    }
}