using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scraper.Net;
using Scraper.Net.Twitter;

namespace TelegramReceiver.Tests
{
    [TestClass]
    public class TwitterValidatorTests
    {
        private readonly TwitterValidator _validator;

        public TwitterValidatorTests()
        {
            var rootConfig = new ConfigurationBuilder().AddUserSecrets<TwitterValidator>().Build();

            var config = rootConfig.GetSection("Twitter").Get<TwitterConfig>();
            
            var scraperService = new ServiceCollection()
                .AddScraper(builder => builder.AddTwitter(config))
                .BuildServiceProvider()
                .GetRequiredService<IScraperService>();
            _validator = new TwitterValidator(scraperService);
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