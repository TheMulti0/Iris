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
            Test("UC4x7LYSzgGH-TMKc9J8pwgQ", "UC4x7LYSzgGH-TMKc9J8pwgQ");
        }
        
        [TestMethod]
        public void TestChannelUrl()
        {
            Test("https://www.youtube.com/channel/UCwNPPl_oX8oUtKVMLxL13jg", "UCwNPPl_oX8oUtKVMLxL13jg");
        }
        
        private void Test(string userId, string expected)
        {
            string extracted = _userIdExtractor.Get(userId);
            Assert.IsNotNull(extracted);
            Assert.AreEqual(expected, extracted);
        }
    }
}