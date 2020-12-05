using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdatesConsumer;

namespace TelegramBot.Tests
{
    [TestClass]
    public class FilterRuleTests
    {
        private static Result<TelegramConfig> _config;
        private static TelegramBot _bot;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            ILoggerFactory loggerFactory = LoggerFactory.Create(
                builder => builder.AddTestsLogging(context));

            _config = Result<TelegramConfig>.Failure("Default config");
            
            var configsProvider = new MockConfigProvider();
            configsProvider.Configs.Subscribe(result => _config = result);

            _bot = new TelegramBot(
                configsProvider,
                new MessageBuilder(new TelegramConfig
                {
                    FilterRules = new []
                    {
                        new FilterRule
                        {
                            UserNames = new [] { "mock-user" },
                            DisableMedia = true,
                            HideMessagePrefix = true
                        }
                    }
                }),
                new MockTelegramBotClientProvider(loggerFactory),
                loggerFactory);
        }
        
        [TestMethod]
        public async Task Test()
        {
            User user = GetFirstConfiguredUser();

            await _bot.OnUpdateAsync(
                new Update
                {
                    AuthorId = user.UserNames[0],
                    Content = "Mock update",
                    Media = new List<IMedia>
                    {   
                        new Photo
                        {
                            Url = "https://mock-photo-url.com"
                        } 
                    }
                }, "test");
            
            await _bot.FlushAsync();
        }
        
        private static User GetFirstConfiguredUser()
        {
            if (_config.IsFailure)
            {
                throw new Exception("Developer exception. Mock config should never be null");
            }

            return _config.Value.Users.FirstOrDefault();
        }
    }
}