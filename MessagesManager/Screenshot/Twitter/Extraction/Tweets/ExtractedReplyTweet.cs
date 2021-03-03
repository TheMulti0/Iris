using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using static MessagesManager.TweetConstants;

namespace MessagesManager
{
    internal class ExtractedReplyTweet : IExtractedTweet
    {
        private readonly IWebDriver _driver;
        private readonly string _tweetContent;
        private readonly WebDriverWait _tweetWait;
        private string _replyTweetXPath;

        public ExtractedReplyTweet(
            IWebDriver driver,
            string tweetContent,
            WebDriverWait tweetWait)
        {
            _driver = driver;
            _tweetContent = tweetContent;
            _tweetWait = tweetWait;
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

                    string cleanText = _tweetContent.CleanText();
                    string objA = text.CleanText();
                    if (!string.Equals(objA, cleanText))
                    {
                        continue;
                    }

                    break;
                }
                catch
                {
                }
                
            }
            
            return element;
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