using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TelegramSender.Tests
{
    [TestClass]
    public class MockTelegramBotTests
    {
        private const string UpdateUrl = "https://mock-url.com";
        private const string PhotoUrl = "https://mock-photo-url.com";
        private const string VideoUrl = "https://mock-video-url.com";
        private const string AudioUrl = "https://mock-audio-url.com";

        private static readonly User User = new(
            "mock-user",
            Platform.Facebook);

        private static MessagesConsumer _consumer;
        private static TestConfig _testConfig;
        private readonly UserChatSubscription _userChatSubscription = new(){ ChatId = _testConfig.ChatId };

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            ILoggerFactory loggerFactory = LoggerFactory.Create(
                builder => builder.AddTestsLogging(context));

            _testConfig = JsonSerializer.Deserialize<TestConfig>("../../../appsettings.json");

            _consumer = new MessagesConsumer(
                GetSenderFactory(_testConfig, loggerFactory),
                new MessageBuilder(new Languages { Dictionary = new Dictionary<Language, LanguageDictionary> {
                    {Language.English, new LanguageDictionary()},
                    {Language.Hebrew, new LanguageDictionary()}
                } }),
                loggerFactory);
        }

        private static ISenderFactory GetSenderFactory(
            TestConfig config,
            ILoggerFactory loggerFactory)
        {
            if (config.Telegram == null)
            {
                return new MockSenderFactory(loggerFactory);
            }
            return new SenderFactory(config.Telegram, loggerFactory);
        }

        [TestMethod]
        public async Task TestText()
        {
            var update = new Update
            {
                Author = User,
                Content = "Mock update"
            };
            
            await _consumer.ConsumeAsync(
                new Message(
                    update,
                    new List<UserChatSubscription>
                    {
                        _userChatSubscription
                    }),
                CancellationToken.None);
            
            await _consumer.FlushAsync();
        }
        
        [TestMethod]
        public async Task TestLongText()
        {
            var content = "";
            for (var i = 0; i < 5000; i++)
            {
                content += $"{i} \n";
            }
            
            var update = new Update
            {
                Author = User,
                Content = content
            };
            
            await _consumer.ConsumeAsync(
                new Message(
                    update,
                    new List<UserChatSubscription> { _userChatSubscription }),
                CancellationToken.None);
            
            await _consumer.FlushAsync();
        }
        
        [TestMethod]
        public async Task TestTextWithUrl()
        {
            var update = new Update
            {
                Author = User,
                Content = "Mock update",
                Url = UpdateUrl
            };
            
            await _consumer.ConsumeAsync(
                new Message(
                    update,
                    new List<UserChatSubscription> { _userChatSubscription }),
                CancellationToken.None);
            
            await _consumer.FlushAsync();
        }
        
        [TestMethod]
        public async Task TestAudio()
        {
            var update = new Update
            {
                Author = User,
                Media = new List<IMedia> 
                {   
                    new Audio(
                        AudioUrl,
                        string.Empty,
                        TimeSpan.FromMinutes(1),
                        "Title",
                        "Artist")
                }
            };
            
            await _consumer.ConsumeAsync(
                new Message(
                    update,
                    new List<UserChatSubscription> { _userChatSubscription }),
                CancellationToken.None);
            
            await _consumer.FlushAsync();
        }
        
        [TestMethod]
        public async Task TestPhoto()
        {
            var update = new Update
            {
                Author = User,
                Media = new List<IMedia>
                {   
                    new Photo(PhotoUrl)
                }
            };
            
            await _consumer.ConsumeAsync(
                new Message(
                    update,
                    new List<UserChatSubscription> { _userChatSubscription }),
                CancellationToken.None);
            
            await _consumer.FlushAsync();
        }
        
        [TestMethod]
        public async Task TestPhotoWithDetails()
        {
            var update = new Update
            {
                Author = User,
                Media = new List<IMedia>
                {   
                    new Photo(PhotoUrl)
                },
                Content = "Mock photo",
                Url = UpdateUrl
            };
            
            await _consumer.ConsumeAsync(
                new Message(
                    update,
                    new List<UserChatSubscription> { _userChatSubscription }),
                CancellationToken.None);

            await _consumer.FlushAsync();
        }
        
        [TestMethod]
        public async Task TestVideo()
        {
            var update = new Update
            {
                Author = User,
                Media = new List<IMedia>
                {
                    new Video(
                        VideoUrl,
                        string.Empty)
                }
            };
            
            await _consumer.ConsumeAsync(
                new Message(
                    update,
                    new List<UserChatSubscription> { _userChatSubscription }),
                CancellationToken.None);
            
            await _consumer.FlushAsync();
        }
        
        [TestMethod]
        public async Task TestVideoWithDetails()
        {
            var update = new Update
            {
                Author = User,
                Media = new List<IMedia>
                {   
                    new Video(
                        VideoUrl,
                        string.Empty
                        )
                },
                Content = "Mock video",
                Url = UpdateUrl
            };
            
            await _consumer.ConsumeAsync(
                new Message(
                    update,
                    new List<UserChatSubscription> { _userChatSubscription }),
                CancellationToken.None);
            
            await _consumer.FlushAsync();
        }
        
        [TestMethod]
        public async Task TestMultipleMediaWithDetails()
        {
            var update = new Update
            {
                Author = User,
                Media = new List<IMedia> 
                {   
                    new Video(
                        VideoUrl,
                        string.Empty
                        ),
                    new Photo(PhotoUrl) 
                },
                Content = "Mock medias",
                Url = UpdateUrl
            };
            
            await _consumer.ConsumeAsync(
                new Message(
                    update,
                    new List<UserChatSubscription> { _userChatSubscription }),
                CancellationToken.None);
            
            await _consumer.FlushAsync();
        }}
}