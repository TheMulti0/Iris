using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using static MessagesManager.TweetConstants;

namespace MessagesManager
{
    internal class ExtractedMediaTweet : IExtractedTweet
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _tweetWait;
        private readonly WebDriverWait _shortWait;

        public ExtractedMediaTweet(
            IWebDriver driver,
            WebDriverWait tweetWait)
        {
            _driver = driver;
            _tweetWait = tweetWait;
            _shortWait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
        }

        public IWebElement GetTweet()
        {
            var element = _tweetWait.Until(_ => _driver.FindElement(By.XPath(TweetXPath)));

            _shortWait.Until(_ => GetThumbnails()); // Wait for tweet thumbnails
            
            return element;
        }

        private IWebElement GetThumbnails()
        {
            return _driver.FindElement(By.XPath(TweetImagesXPath));
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