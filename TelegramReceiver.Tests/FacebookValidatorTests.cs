using System.Threading.Tasks;
using FacebookScraper;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TelegramReceiver.Tests
{
    [TestClass]
    public class FacebookValidatorTests
    {
        private readonly FacebookValidator _validator = new(new FacebookUpdatesProvider(NullLogger<FacebookUpdatesProvider>.Instance));
        
        [TestMethod]
        public Task TestUserName()
        {
            return Test("NaftaliBennett");
        }
        
        [TestMethod]
        public Task TestProfileUrl()
        {
            return Test("https://www.facebook.com/NaftaliBennett");
        }

        [TestMethod]
        public Task TestMobileProfileUrl()
        {
            return Test("https://m.facebook.com/NaftaliBennett");
        }

        [TestMethod]
        public Task TestComplicatedProfileUrl()
        {
            return Test("https://www.facebook.com/%D7%A8%D7%95%D7%A0%D7%99-%D7%A1%D7%A1%D7%95%D7%91%D7%A8-Roni-Sassover-100178875444889");
        }

        [TestMethod]
        public Task TestPostUrl()
        {
            return Test("https://www.facebook.com/NaftaliBennett/videos/1511862265871708");
        }

        private async Task Test(string userId)
        {
            Assert.IsNotNull(await _validator.ValidateAsync(userId));
        }
    }
}