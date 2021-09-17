using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TelegramReceiver.Tests
{
    [TestClass]
    public class FeedsValidatorTests
    {
        private readonly FeedsUserIdExtractor _userIdExtractor;

        public FeedsValidatorTests()
        {
            _userIdExtractor = new FeedsUserIdExtractor();
        }
        
        [TestMethod]
        public void Test()
        {
            Assert.IsNotNull(_userIdExtractor.Get("https"));
            Assert.IsNull(_userIdExtractor.Get("ht"));
        }
    }
}