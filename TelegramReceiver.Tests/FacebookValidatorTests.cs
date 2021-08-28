using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TelegramReceiver.Tests
{
    [TestClass]
    public class FacebookValidatorTests
    {
        private readonly FacebookUserIdExtractor _userIdExtractor;

        public FacebookValidatorTests()
        {
            _userIdExtractor = new FacebookUserIdExtractor();
        }
        
        [TestMethod]
        public void TestUserName()
        {
            Test("NaftaliBennett");
        }
        
        [TestMethod]
        public void TestProfileUrl()
        {
            Test("https://www.facebook.com/NaftaliBennett");
        }

        [TestMethod]
        public void TestMobileProfileUrl()
        {
            Test("https://m.facebook.com/NaftaliBennett");
        }

        [TestMethod]
        public void TestComplicatedProfileUrl()
        {
            Test("https://www.facebook.com/%D7%A8%D7%95%D7%A0%D7%99-%D7%A1%D7%A1%D7%95%D7%91%D7%A8-Roni-Sassover-100178875444889");
        }

        [TestMethod]
        public void TestPostUrl()
        {
            Test("https://www.facebook.com/NaftaliBennett/videos/1511862265871708");
        }

        private void Test(string userId)
        {
            Assert.IsNotNull(_userIdExtractor.Get(userId));
        }
    }
}