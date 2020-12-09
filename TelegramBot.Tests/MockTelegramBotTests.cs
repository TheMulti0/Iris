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
    public class MockTelegramBotTests
    {
        private const string UpdateUrl = "https://mock-url.com";
        private const string PhotoUrl = "https://mock-photo-url.com";
        private const string VideoUrl = "https://mock-video-url.com";
        private const string AudioUrl = "https://mock-audio-url.com";
        
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
                new MessageBuilder(new TelegramConfig()),
                new MockTelegramBotClientProvider(loggerFactory),
                loggerFactory);
        }
        
        [TestMethod]
        public async Task TestText()
        {
            User user = GetFirstConfiguredUser();

            await _bot.OnUpdateAsync(
                new Update
                {
                    AuthorId = user.UserNames[0],
                    Content = "Mock update"
                });
            
            await _bot.FlushAsync();
        }
        
        [TestMethod]
        public async Task TestTextWithUrl()
        {
            User user = GetFirstConfiguredUser();

            await _bot.OnUpdateAsync(
                new Update
                {
                    AuthorId = user.UserNames[0],
                    Content = "Mock update",
                    Url = UpdateUrl
                });
            
            await _bot.FlushAsync();
        }
        
        [TestMethod]
        public async Task TestAudio()
        {
            User user = GetFirstConfiguredUser();

            await _bot.OnUpdateAsync(
                new Update
                {
                    AuthorId = user.UserNames[0],
                    Media = new List<IMedia> 
                    {   
                        new Audio(
                            AudioUrl,
                            string.Empty,
                            TimeSpan.FromMinutes(1),
                            "Title",
                            "Artist")
                    }
                });
            
            await _bot.FlushAsync();
        }
        
        [TestMethod]
        public async Task TestPhoto()
        {
            User user = GetFirstConfiguredUser();

            await _bot.OnUpdateAsync(
                new Update
                {
                    AuthorId = user.UserNames[0],
                    Media = new List<IMedia>
                    {   
                        new Photo(PhotoUrl)
                    }
                });
            
            await _bot.FlushAsync();
        }
        
        [TestMethod]
        public async Task TestPhotoWithDetails()
        {
            User user = GetFirstConfiguredUser();

            await _bot.OnUpdateAsync(
                new Update
                {
                    AuthorId = user.UserNames[0],
                    Media = new List<IMedia>
                    {   
                        new Photo(PhotoUrl)
                    },
                    Content = "Mock photo",
                    Url = UpdateUrl
                });
            
            
            await _bot.FlushAsync();
        }
        
        [TestMethod]
        public async Task TestVideo()
        {
            User user = GetFirstConfiguredUser();

            await _bot.OnUpdateAsync(
                new Update
                {
                    AuthorId = user.UserNames[0],
                    Media = new List<IMedia>
                    {
                        new Video(
                            VideoUrl,
                            string.Empty,
                            true)
                    }
                });
            
            await _bot.FlushAsync();
        }
        
        [TestMethod]
        public async Task TestVideoWithDetails()
        {
            User user = GetFirstConfiguredUser();

            await _bot.OnUpdateAsync(
                new Update
                {
                    AuthorId = user.UserNames[0],
                    Media = new List<IMedia>
                    {   
                        new Video(
                            VideoUrl,
                            string.Empty,
                            true
                        )
                    },
                    Content = "Mock video",
                    Url = UpdateUrl
                });
            
            await _bot.FlushAsync();
        }
        
        [TestMethod]
        public async Task TestMultipleMediaWithDetails()
        {
            User user = GetFirstConfiguredUser();

            await _bot.OnUpdateAsync(
                new Update
                {
                    AuthorId = user.UserNames[0],
                    Media = new List<IMedia> 
                    {   
                        new Video(
                            VideoUrl,
                            string.Empty,
                            true
                        ),
                        new Photo(PhotoUrl) 
                    },
                    Content = "Mock medias",
                    Url = UpdateUrl
                });
            
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