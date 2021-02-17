using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tweetinvi.Models;
using IMedia = Common.IMedia;

namespace TwitterScraper.Tests
{
    [TestClass]
    public class TwitterUpdatesProviderTests
    {
        private readonly TwitterUpdatesProvider _twitter;

        public TwitterUpdatesProviderTests()
        {
            var rootConfig = new ConfigurationBuilder().AddUserSecrets<TwitterUpdatesProviderConfig>().Build();

            var config = rootConfig.GetSection<TwitterUpdatesProviderConfig>("UpdatesProvider");
            
            _twitter = new TwitterUpdatesProvider(config);
        }

        [TestMethod]
        public async Task TestTheMulti0()
        {
            List<Update> updates = (await _twitter.GetUpdatesAsync(new User("themulti0", Platform.Twitter))).ToList();
            
            Assert.IsNotNull(updates);
            CollectionAssert.AllItemsAreNotNull(updates);
        }
        
        [TestMethod]
        public void TestCleanTextWithShortenedUrl()
        {
            const string url = "https://t.co/Po1PsWNB49";
            
            var expanded = _twitter.CleanText(url);
            
            Assert.IsFalse(expanded.Contains(url));
        }
        
        [TestMethod]
        public void TestCleanTextWithPictureShortenedUrl()
        {
            const string url = "https://pic.twitter.com/epqvVYkWKC";
            
            var expanded = _twitter.CleanText(url);
            
            Assert.IsFalse(expanded.Contains(url));
        }

        [TestMethod]
        public async Task TestPhotoExtraction()
        {
            const long tweetId = 1361968093686333440;
            
            ITweet tweet = await _twitter.TwitterClient.Tweets.GetTweetAsync(tweetId);

            IEnumerable<IMedia> media = TwitterUpdatesProvider.GetMedia(tweet);
            
            Assert.IsTrue(media.Any(m => m is Photo));
        }

        [TestMethod]
        public async Task TestVideoExtraction()
        {
            const long tweetId = 1361968818323664899;
            
            ITweet tweet = await _twitter.TwitterClient.Tweets.GetTweetAsync(tweetId);

            IEnumerable<IMedia> media = TwitterUpdatesProvider.GetMedia(tweet);
            
            Assert.IsTrue(media.Any(m => m is Video));
        }
    }
}