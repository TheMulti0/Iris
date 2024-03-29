﻿using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scraper.MassTransit.Common;
using Scraper.Net;

namespace TelegramSender.Tests
{
    [TestClass]
    public class MessageInfoBuilderTests
    {
        private readonly MessageInfoBuilder _builder = new();        
        
        [TestMethod]
        public void TestTwitterUserNamesHyperlink()
        {
            var disabledText = new Text
            {
                Enabled = false
            };
            
            var messageInfo = _builder.Build(
                new NewPost
                {
                    Platform = "twitter",
                    Post = new Post
                    {
                        Content = "hyperlink @themulti0 and @realDonaldTrump"
                    }
                },
                new UserChatSubscription
                {
                    Prefix = disabledText,
                    Suffix = disabledText,
                    ChatInfo = new ChatInfo()
                },
                default);
            
            Assert.IsNotNull(messageInfo);
        }
    }
}