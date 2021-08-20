using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scraper.Net;
using Scraper.Net.Facebook;

namespace TelegramReceiver.Tests
{
    [TestClass]
    public class FacebookValidatorTests
    {
        private readonly FacebookValidator _validator;

        public FacebookValidatorTests()
        {
            var scraperService = new ServiceCollection()
                .AddScraper(builder => builder.AddFacebook())
                .BuildServiceProvider()
                .GetRequiredService<IScraperService>();
            _validator = new FacebookValidator(scraperService);
        }
        
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