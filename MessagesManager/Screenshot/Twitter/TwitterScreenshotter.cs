using System;
using System.Drawing;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace MessagesManager
{
    internal class TwitterScreenshotter
    {
        private const string TweetXPath = "/html/body/div/div/div/div[2]/main/div/div/div/div[1]/div/div[2]/div/section/div/div/div[1]/div/div/article";
        private static readonly string TweetImagesXPath = $"{TweetXPath}/div/div/div/div[3]/div[2]/div/div/div/div/div/div[2]/div";
        private static readonly string TweetDateXPath = $"{TweetXPath}/div/div/div/div[3]/div[3]";
        private static readonly string TweetStatsXPath = $"{TweetXPath}/div/div/div/div[3]/div[4]";
        private static readonly string TweetButtonsXPath = $"{TweetXPath}/div/div/div/div[3]/div[5]";

        private const string ReplyTweetXPath = "/html/body/div/div/div/div[2]/main/div/div/div/div/div/div[2]/div/section/div/div/div[2]/div/div/article";
        private static readonly string ReplyTweetDateXPath = $"{ReplyTweetXPath}/div/div/div/div[3]/div[4]";
        private static readonly string ReplyTweetStatsXPath = $"{ReplyTweetXPath}/div/div/div/div[3]/div[5]";
        private static readonly string ReplyTweetButtonsXPath = $"{ReplyTweetXPath}/div/div/div/div[3]/div[6]";

        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        private readonly WebDriverWait _shortWait;

        public TwitterScreenshotter(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            _shortWait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
        }

        public Bitmap Screenshot(string url)
        {
            try
            {
                Setup(url);

                var tweet = GetTweet(out bool isReplyTweet);

                WaitForTweetThumbnails();

                Bitmap bitmap = Screenshot(tweet, isReplyTweet);

                _driver.Dispose();
                
                return bitmap;
            }
            catch
            {
                _driver.Dispose();
                throw;
            }
        }

        private IWebElement GetTweet(out bool isReplyTweet)
        {
            try
            {
                isReplyTweet = false;
                return _wait.Until(GetElement(By.XPath(TweetXPath)));
            }
            catch (WebDriverTimeoutException)
            {
                isReplyTweet = true;
                return _wait.Until(GetElement(By.XPath(ReplyTweetXPath)));
            }
        }

        private void Setup(string url)
        {
            _driver.Manage()
                .Window.Size = new Size(1080, 2400);

            _driver.Navigate()
                .GoToUrl(url);
        }

        private void WaitForTweetThumbnails()
        {
            try
            {
                _shortWait.Until(GetElement(By.XPath(TweetImagesXPath))); // Wait for tweet thumbnails
            }
            catch (WebDriverTimeoutException)
            {
                // Tweet might be text so no images will be loaded
            }
        }

        private Bitmap Screenshot(IWebElement tweet, bool isReplyTweet)
        {
            var takesScreenshot = (ITakesScreenshot) tweet;
            
            return takesScreenshot
                .GetScreenshot()
                .AsByteArray
                .ToBitmap()
                .Crop(GetViewport(tweet.Size, isReplyTweet))
                .RoundCorners(30);
        }

        private Rectangle GetViewport(Size tweetSize, bool isReplyTweet)
        {
            (int dateHeight, int statsHeight, int buttonsHeight) = GetHeights(isReplyTweet);

            var finalHeight = new Size(
                tweetSize.Width,
                tweetSize.Height - dateHeight - statsHeight - buttonsHeight);

            return new Rectangle(Point.Empty, finalHeight);
        }

        private (int dateHeight, int statsHeight, int buttonsHeight) GetHeights(bool isReplyTweet)
        {
            int dateHeight;
            int statsHeight;
            int buttonsHeight;
            
            if (!isReplyTweet)
            {
                try
                {
                    (dateHeight, statsHeight, buttonsHeight) = GetTweetHeights();
                }
                catch (NoSuchElementException)
                {
                    (dateHeight, statsHeight, buttonsHeight) = GetReplyTweetHeights();
                }
            }
            else
            {
                (dateHeight, statsHeight, buttonsHeight) = GetReplyTweetHeights();
            }
            
            return (dateHeight + 20, statsHeight, buttonsHeight);
        }

        private (int dateHeight, int statsHeight, int buttonsHeight) GetTweetHeights()
        {
            int dateHeight = _driver.FindElement(By.XPath(TweetDateXPath)).Size.Height;
            int statsHeight = _driver.FindElement(By.XPath(TweetStatsXPath)).Size.Height;
            int buttonsHeight = _driver.FindElement(By.XPath(TweetButtonsXPath)).Size.Height;
            
            return (dateHeight, statsHeight, buttonsHeight);
        }

        private (int dateHeight, int statsHeight, int buttonsHeight) GetReplyTweetHeights()
        {
            int dateHeight;
            int statsHeight;
            int buttonsHeight;
            dateHeight = _driver.FindElement(By.XPath(ReplyTweetDateXPath))
                .Size.Height + 20;
            statsHeight = _driver.FindElement(By.XPath(ReplyTweetStatsXPath))
                .Size.Height;
            buttonsHeight = _driver.FindElement(By.XPath(ReplyTweetButtonsXPath))
                .Size.Height;
            return (dateHeight, statsHeight, buttonsHeight);
        }

        private static Func<IWebDriver, IWebElement> GetElement(By by) 
            => driver => driver.FindElement(by);
    }
}