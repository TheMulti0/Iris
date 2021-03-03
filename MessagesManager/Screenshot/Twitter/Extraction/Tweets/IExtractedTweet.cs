using OpenQA.Selenium;

namespace MessagesManager
{
    internal interface IExtractedTweet
    {
        IWebElement GetTweet();
        
        TweetHeights GetHeights();
    }
}