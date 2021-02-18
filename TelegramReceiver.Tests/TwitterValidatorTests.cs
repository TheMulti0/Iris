using System.Threading.Tasks;
using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TwitterScraper;

namespace TelegramReceiver.Tests
{
    [TestClass]
    public class TwitterValidatorTests
    {
        private readonly TwitterValidator _validator;

        public TwitterValidatorTests()
        {
            var rootConfig = new ConfigurationBuilder().AddUserSecrets<TwitterValidator>().Build();

            var config = rootConfig.GetSection<TwitterUpdatesProviderConfig>("Twitter");
            
            _validator = new TwitterValidator(new TwitterUpdatesProvider(config));
        }
        
        [TestMethod]
        public Task TestUserName()
        {
            return Test("themulti0");
        }
        
        [TestMethod]
        public Task TestTagUserName()
        {
            return Test("@themulti0");
        }
        
        [TestMethod]
        public Task TestProfileUrl()
        {
            return Test("https://www.twitter.com/themulti0");
        }
        
        [TestMethod]
        public Task TestTweetUrl()
        {
            return Test("https://twitter.com/themulti0/status/1362469986724364288");
        }

        private async Task Test(string userId)
        {
            Assert.IsNotNull(await _validator.ValidateAsync(userId));
        }
    }
}