using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using static MessagesManager.TweetConstants;

namespace MessagesManager
{
    internal class ExtractedTweet : IExtractedTweet
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _tweetWait;

        public ExtractedTweet(
            IWebDriver driver,
            WebDriverWait tweetWait)
        {
            _driver = driver;
            _tweetWait = tweetWait;
        }

        public IWebElement GetTweet()
        {
            return _tweetWait.Until(_ => _driver.FindElement(By.XPath(TweetXPath)));
        }

        public TweetHeights GetHeights()
        {
            int dateHeight = _driver.FindElement(By.XPath(TweetDateXPath)).Size.Height;
            int statsHeight = _driver.FindElement(By.XPath(TweetStatsXPath)).Size.Height;
            int buttonsHeight = _driver.FindElement(By.XPath(TweetButtonsXPath)).Size.Height;
            
            return new TweetHeights(dateHeight, statsHeight, buttonsHeight);
        }
    }
}