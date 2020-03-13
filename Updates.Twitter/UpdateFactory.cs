using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Primitives;
using Tweetinvi.Logic;
using Tweetinvi.Models;
using Updates.Api;

namespace Updates.Twitter
{
    internal static class UpdateFactory
    {
        public static Update ToUpdate(ITweet tweet)
        {
            long id = tweet.Id;
            var author = UserFactory.ToUser(tweet.CreatedBy);
            string message = tweet.Text;
            string formattedMessage = GetFormattedMessage(tweet);
            DateTime createdAt = tweet.CreatedAt;
            string url = tweet.Url;
            IEnumerable<Media> media = tweet.Media?
                .Select(MediaFactory.ToMedia) ?? new List<Media>();

            return new Update(
                id,
                author,
                message,
                formattedMessage,
                createdAt,
                url,
                media);
        }

        private static string GetFormattedMessage(ITweet tweet)
        {
            const string header = "ציוץ חדש פורסם כעת מאת";
            StringBuilder builder = new StringBuilder($"{header}:\n")
                .Append(GetTweetText(tweet));

            if (tweet.QuotedTweet != null)
            {
                builder.Append(
                    "\n הציוץ הזה הוא תגובה לציוץ הבא מאת: \n" +
                    GetTweetText(tweet.QuotedTweet));
            }

            builder.Append(
                "\n \n \n \n" +
                $"{tweet.Url}");
            
            return builder.ToString();
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