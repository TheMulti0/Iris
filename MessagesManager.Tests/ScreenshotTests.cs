using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MessagesManager.Tests
{
    [TestClass]
    public class ScreenshotTests
    {
        private readonly Screenshotter _screenshotter;

        public ScreenshotTests()
        {
            _screenshotter = new Screenshotter();
        }
        
        [TestMethod]
        public async Task TestTweetScreenshot()
        {
            var update = new Update
            {
                Url = "https://twitter.com/bezalelsm/status/1349617962643300352",
                Author = new User(string.Empty, Platform.Twitter)
            };
            
            byte[] screenshot = await _screenshotter.ScreenshotAsync(update);
            
            Assert.IsTrue(screenshot.Any());

            await File.WriteAllBytesAsync("../../../output.jpg", screenshot);
        }
    }
}