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
            string message = tweet.FullText;
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
            StringBuilder builder;
            
            if (tweet.IsRetweet)
            {
                builder = new StringBuilder(FormatHeader("פורסם ציוץ מחדש כעת מעת"));
                builder.Append($"{GetAuthorName(tweet.CreatedBy)} \n");
                builder.Append(
                    "\n הציוץ המקורי מאת: \n" +
                    GetTweetText(tweet.RetweetedTweet));
            }
            else
            {
                builder = new StringBuilder(FormatHeader("ציוץ חדש פורסם כעת מאת"));
                
                if (tweet.QuotedTweet != null)
                {
                    builder.Append(
                        "\n הציוץ הזה הוא תגובה לציוץ הבא מאת: \n" +
                        GetTweetText(tweet.QuotedTweet));
                }
            }

            builder.Append(
                "\n \n \n \n" +
                $"{tweet.Url}");
            
            return builder.ToString();
        }

        private static string FormatHeader(string header) 
            => $"{header}:\n";

        private static string GetTweetText(ITweet tweet)
        {
            IUser author = tweet.CreatedBy;
            
            return GetAuthorName(author) +
                   "\n \n \n" +
                   $"`\"{tweet.Text}\"`";
        }

        private static string GetAuthorName(IUser author) => $"*{author.Name}* (@{author.ScreenName})";
    }
}