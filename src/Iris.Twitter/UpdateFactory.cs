using System;
using System.Collections.Generic;
using System.Linq;
using Iris.Api;

namespace Iris.Twitter.Factories
{
    internal static class UpdateFactory
    {
        public static Update ToUpdate(this Tweet tweet, User author)
        {
            string url = GetUrl(tweet, author);

            long id = tweet.Id;
            string message = tweet.Text;
            string formattedMessage = GetFormattedMessage(tweet, author, url);
            DateTime createdAt = tweet.Date;
            IEnumerable<Api.Media> media = tweet.Media.ToMedias();

            return new Update(
                id,
                author,
                message,
                formattedMessage,
                createdAt,
                url,
                media);
        }

        private static IEnumerable<Api.Media> ToMedias(this Media media)
        {
            return media.Photos.Select(url => new Api.Media(url, MediaType.Photo));
        }

        private static string GetUrl(Tweet tweet, User author)
        {
            return $"https://twitter.com/{author.Id}/status/{tweet.Id}";
        }

        private static string GetFormattedMessage(Tweet tweet, User author, string url)
        {
            string verb = author.Gender == Gender.Male ? "צייץ" : "צייצה";
            const string retweetVerb = "מחדש";
            string fullVerb = tweet.IsRetweet 
                ? $"{verb} {retweetVerb}" 
                : verb;
            
            return MessageFormatter.FormatMessage(
                url,
                author.Name,
                fullVerb,
                tweet.Text);
        }
    }
}