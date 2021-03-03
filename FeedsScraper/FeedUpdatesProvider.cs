using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeHollow.FeedReader;
using CodeHollow.FeedReader.Feeds;
using CodeHollow.FeedReader.Feeds.Itunes;
using Common;
using UpdatesScraper;

namespace FeedsScraper
{
    public class FeedUpdatesProvider : IUpdatesProvider
    {
        public async Task<IEnumerable<Update>> GetUpdatesAsync(User user)
        {
            // userId is treated as url to the feed
            Feed feed = await FeedReader.ReadAsync(user.UserId);

            return feed.Items.Select(
                ToUpdate(user));
        }

        private Func<FeedItem, Update> ToUpdate(User feed)
        {
            return item => new Update
            {
                Content = GetContent(item),
                Author = feed,
                CreationDate = item.PublishingDate,
                Url = item.Link,
                Media = GetMedia(item),
                IsRepost = false
            };
        }

        private static string GetContent(FeedItem item)
        {
            // Combines content and description and separates them with two line breaks,
            // if one is null then there will be no extra line breaks
            
            string[] values = { item.Content, item.Description };
            
            return string.Join(
                "\n \n",
                values.Where(s => s != null));
        }

        private List<IMedia> GetMedia(FeedItem item)
        {
            var enclosure = GetEnclosure(item.SpecificItem);

            if (enclosure != null)
            {
                ItunesItem itunesItem = item.GetItunesItem();

                if (enclosure.MediaType.StartsWith("audio") &&
                    itunesItem.Duration != null)
                {
                    return new List<IMedia>
                    {
                        new Audio(
                            enclosure.Url,
                            itunesItem.Image.Href,
                            itunesItem.Duration,
                            itunesItem.Summary,
                            itunesItem.Author)
                    };
                }
            }

            return new List<IMedia>();
        }

        private FeedItemEnclosure GetEnclosure(BaseFeedItem baseItem)
        {
            return baseItem switch 
            {
                MediaRssFeedItem item => item.Enclosure,
                Rss20FeedItem item => item.Enclosure,
                _ => null
            };
        }
    }
}