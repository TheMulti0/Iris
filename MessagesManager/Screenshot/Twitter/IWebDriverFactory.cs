using OpenQA.Selenium;

namespace MessagesManager
{
    internal interface IWebDriverFactory
    {
        IWebDriver Create();
    }
}