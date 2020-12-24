using System.Threading.Tasks;
using NUnit.Framework;

namespace TwitterScraper.Tests
{
    public class UrlExpanderTests
    {
        private readonly UrlExpander _expander;

        public UrlExpanderTests()
        {
            _expander = new UrlExpander();
        }

        [Test]
        public async Task TestTwitterShortenedUrl()
        {
            const string url = "https://t.co/QOSO1d4kY3";
            
            var expanded = await _expander.ExpandAsync(url);
            
            Assert.IsNotNull(expanded);
            Assert.AreNotEqual(url, expanded);
        }
        
        [Test]
        public async Task TestTwitterShortenedFacebookPostUrl()
        {
            const string url = "https://t.co/Po1PsWNB49";
            
            var expanded = await _expander.ExpandAsync(url);
            
            Assert.IsNotNull(expanded);
            Assert.AreEqual(url, expanded);
        }
    }
}