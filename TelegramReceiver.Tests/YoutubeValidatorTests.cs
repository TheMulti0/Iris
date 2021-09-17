using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TelegramReceiver.Tests
{
    [TestClass]
    public class YoutubeValidatorTests
    {
        private readonly YoutubeUserIdExtractor _userIdExtractor;

        public YoutubeValidatorTests()
        {
            _userIdExtractor = new YoutubeUserIdExtractor();
        }
        
        [TestMethod]
        public void TestChannelId()
        {
            Test("UCwNPPl_oX8oUtKVMLxL13jg");
        }
        
        [TestMethod]
        public void TestChannelUrl()
        {
            Test("https://www.youtube.com/channel/UCwNPPl_oX8oUtKVMLxL13jg");
        }
        
        private void Test(string userId)
        {
            Assert.IsNotNull(_userIdExtractor.Get(userId));
        }
    }
}