using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TwitterScraper.Tests
{
    [TestClass]
    public class UrlExpanderTests
    {
        private readonly UrlExpander _expander;

        public UrlExpanderTests()
        {
            _expander = new UrlExpander();
        }

        [TestMethod]
        public async Task TestTwitterShortenedUrl()
        {
            const string url = "https://t.co/QOSO1d4kY3";
            
            var expanded = await _expander.ExpandAsync(url);
            
            Assert.IsNotNull(expanded);
            Assert.AreNotEqual(url, expanded);
        }
        
        [TestMethod]
        public async Task TestTwitterShortenedFacebookPostUrl()
        {
            const string url = "https://t.co/Po1PsWNB49";
            
            var expanded = await _expander.ExpandAsync(url);
            
            Assert.IsNotNull(expanded);
            Assert.AreEqual(url, expanded);
        }
    }
}