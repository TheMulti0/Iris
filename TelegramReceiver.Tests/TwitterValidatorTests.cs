using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TelegramReceiver.Tests
{
    [TestClass]
    public class TwitterValidatorTests
    {
        private readonly TwitterUserIdExtractor _userIdExtractor;

        public TwitterValidatorTests()
        {
            _userIdExtractor = new TwitterUserIdExtractor();
        }
        
        [TestMethod]
        public void TestUserName()
        {
            Test("themulti0");
        }
        
        [TestMethod]
        public void TestTagUserName()
        {
            Test("@themulti0");
        }
        
        [TestMethod]
        public void TestProfileUrl()
        {
            Test("https://www.twitter.com/themulti0");
        }
        
        [TestMethod]
        public void TestTweetUrl()
        {
            Test("https://twitter.com/themulti0/status/1362469986724364288");
        }

        private void Test(string userId)
        {
            Assert.IsNotNull(_userIdExtractor.Get(userId));
        }
    }
}