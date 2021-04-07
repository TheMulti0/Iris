using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using static MessagesManager.TweetConstants;

namespace MessagesManager
{
    internal class ExtractedReplyMediaTweet : IExtractedTweet
    {
        private readonly IWebDriver _driver;
        private readonly string _tweetContent;
        private readonly WebDriverWait _tweetWait;
        private readonly WebDriverWait _shortWait;
        private string _replyTweetXPath;

        public ExtractedReplyMediaTweet(
            IWebDriver driver,
            string tweetContent,
            WebDriverWait tweetWait)
        {
            _driver = driver;
            _tweetContent = tweetContent;
            _tweetWait = tweetWait;
            _shortWait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
        }

        public IWebElement GetTweet()
        {
            IWebElement element = null;

            for (int i = 2; i < 20; i++)
            {
                try
                {
                    _replyTweetXPath = GetReplyTweetXPath(i);
                    element = _tweetWait.Until(_ => _driver.FindElement(By.XPath(_replyTweetXPath)));

                    var text = _driver.FindElement(By.XPath(GetReplyTweetTextXPath(_replyTweetXPath))).Text;

                    if (!text.CleanText().StartsWith(_tweetContent.CleanText()))
                    {
                        continue;
                    }

                    _shortWait.Until(_ => GetThumbnails()); // Wait for tweet thumbnails
                    
                    break;
                }
                catch
                {
                }
            }
            
            return element;
        }
        
        private IWebElement GetThumbnails()
        {
            return _driver.FindElement(By.XPath(GetReplyTweetImagesXPath(_replyTweetXPath)));
        }

        public TweetHeights GetHeights()
        {
            int dateHeight = _driver.FindElement(By.XPath(GetReplyTweetDateXPath(_replyTweetXPath))).Size.Height;
            int statsHeight = _driver.FindElement(By.XPath(GetReplyTweetStatsXPath(_replyTweetXPath))).Size.Height;
            int buttonsHeight = _driver.FindElement(By.XPath(GetReplyTweetButtonsXPath(_replyTweetXPath))).Size.Height;
            
            return new TweetHeights(dateHeight, statsHeight, buttonsHeight);
        }
    }
}