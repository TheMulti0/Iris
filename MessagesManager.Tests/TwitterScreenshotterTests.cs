using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MessagesManager.Tests
{
    [TestClass]
    public class TwitterScreenshotterTests
    {
        private readonly Func<TwitterScreenshotter> _screenshotterFactory;

        public TwitterScreenshotterTests()
        {
            _screenshotterFactory = () => new TwitterScreenshotter(new WebDriverFactory(new TwitterScreenshotterConfig{ UseLocalChromeDriver = true }).Create());
        }
        
        [TestMethod]
        public void TestTextTweet()
        {
            Test("https://twitter.com/IsraelPolls/status/1362480543733014537");
        }
        
        [TestMethod]
        public void TestAlbumTweet()
        {
            Test("https://twitter.com/yairlapid/status/1362479265762189313");
        }
        
        [TestMethod]
        public void TestVideoTweet()
        {
            Test("https://twitter.com/Hatzehaka/status/1362481354483597316");
        }
        
        [TestMethod]
        public void TestReplyTweet()
        {
            Test("https://twitter.com/ronisassover/status/1363384809339379712");
        }

        [TestMethod]
        public void TestQuoteTweet()
        {
            Test("https://twitter.com/bezalelsm/status/1363360010298875907");
        }

        private void Test(string url)
        {
            var screenshot = _screenshotterFactory().Screenshot(url);

            Assert.IsNotNull(screenshot);
        }
    }
}