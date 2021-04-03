using System.Threading.Tasks;
using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TdLib;

namespace TelegramClient.Tests
{
    [TestClass]
    public class TelegramClientTests
    {
        private const string Text = "test";
        private const string PhotoUrl = "https://images.unsplash.com/photo-1529736576495-1ed4a29ca7e1?ixlib=rb-1.2.1&ixid=MXwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHw%3D&auto=format&fit=crop&w=752&q=80";

        private static ITelegramClient _client;
        private static long _chatId;

        [ClassInitialize]
        public static async Task Initialize(TestContext context)
        {
            var rootConfig = new ConfigurationBuilder().AddUserSecrets<TelegramClientTests>().Build();

            var config = rootConfig.GetSection<TelegramClientConfig>("TelegramClientConfig");
            var factory = new TelegramClientFactory(config);
            
            _client = await factory.CreateAsync();
            
            var chatId = rootConfig.GetValue<long>("TelegramClientTestChatId");
            _chatId = (await _client.GetChatAsync(chatId)).Id;
        }
        
        [TestMethod]
        public Task TestTextMessage()
        {
            return TestSendMessage(new TdApi.InputMessageContent.InputMessageText
            {
                Text = new TdApi.FormattedText
                {
                    Text = Text
                }
            });
        }

        [TestMethod]
        public Task TestUrlPhotoMessage()
        {
            return TestSendMessage(
                new TdApi.InputMessageContent.InputMessagePhoto
                {
                    Photo = new TdApi.InputFile.InputFileRemote
                    {
                        Id = PhotoUrl
                    }
                });
        }
        private static async Task TestSendMessage(TdApi.InputMessageContent messageContent)
        {
            var message = await _client.SendMessageAsync(
                _chatId,
                messageContent);

            Assert.IsNotNull(message);
        }
    }
}