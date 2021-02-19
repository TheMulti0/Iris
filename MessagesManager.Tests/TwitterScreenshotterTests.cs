using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MessagesManager.Tests
{
    [TestClass]
    public class TwitterScreenshotterTests
    {
        private readonly TwitterScreenshotter _screenshotter;

        public TwitterScreenshotterTests()
        {
            _screenshotter = new TwitterScreenshotter(new WebDriverFactory(new TwitterScreenshotterConfig{ UseLocalChromeDriver = true }).Create());
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

        private void Test(string url)
        {
            var screenshot = _screenshotter.Screenshot(url);

            Assert.IsNotNull(screenshot);
        }
    }
}