using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdatesConsumer;

namespace TelegramBot.Tests
{
    [TestClass]
    public class MockTelegramBotTests
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
                }, "test");
            
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
                    Url = "https://mock-url.com"
                }, "test");
            
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
                        new Audio
                        {
                            Url = "https://awaod01.streamgates.net/103fm_aw/nis1109206.mp3?aw_0_1st.collectionid=nis&aw_0_1st.episodeid=109206&aw_0_1st.skey=1599814244&listeningSessionID=5f159c950b71b138_191_254__54fddcd17821d4ada536bb55cbcd9a3084e57e35",
                            DurationSeconds = 60,
                            Title = "Title",
                            Artist = "Artist"
                        } 
                    }
                }, "test");
            
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
                        new Photo
                        {
                            Url = "https://mock-photo-url.com"
                        } 
                    }
                }, "test");
            
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
                        new Photo
                        {
                            Url = "https://mock-photo-url.com"
                        } 
                    },
                    Content = "Mock photo",
                    Url = "https://mock-url.com"
                }, "test");
            
            
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
                        new Video
                        {
                            Url = "https://mock-video-url.com"
                        } 
                    }
                }, "test");
            
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
                        new Video
                        {
                            Url = "https://mock-video-url.com"
                        } 
                    },
                    Content = "Mock video",
                    Url = "https://mock-url.com"
                }, "test");
            
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
                        new Video
                        {
                            Url = "https://mock-video-url.com"
                        },
                        new Photo
                        {
                            Url = "https://mock-photo-url.com"
                        } 
                    },
                    Content = "Mock medias",
                    Url = "https://mock-url.com"
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