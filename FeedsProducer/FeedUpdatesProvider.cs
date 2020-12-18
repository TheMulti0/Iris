using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeHollow.FeedReader;
using CodeHollow.FeedReader.Feeds;
using CodeHollow.FeedReader.Feeds.Itunes;
using Common;
using UpdatesProducer;

namespace FeedsProducer
{
    public class FeedUpdatesProvider : IUpdatesProvider
    {
        private readonly UpdatesProviderBaseConfig _config;

        public FeedUpdatesProvider(
            UpdatesProviderBaseConfig config)
        {
            _config = config;
        }
        
        public async Task<IEnumerable<Update>> GetUpdatesAsync(string userId)
        {
            // userId is treated as url to the feed
            Feed feed = await FeedReader.ReadAsync(userId);

            return feed.Items.Select(
                ToUpdate(
                    feed.Title,
                    _config.Name));
        }

        private Func<FeedItem, Update> ToUpdate(string feedName, string source)
        {
            return item => new Update
            {
                Content = GetContent(item),
                AuthorId = feedName,
                CreationDate = item.PublishingDate,
                Url = item.Link,
                Media = GetMedia(item),
                Repost = false,
                Source = source
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