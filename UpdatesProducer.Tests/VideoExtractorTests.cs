using System;
using System.Threading.Tasks;
using Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UpdatesProducer.Tests
{
    [TestClass]
    public class VideoExtractorTests
    {
        [TestMethod]
        public async Task TestExtract()
        {
            const string url = "https://www.facebook.com/EretzNehederet.Keshet/videos/1254084181637623";

            var video = await VideoExtractor.ExtractVideo(url);
            
            Assert.IsNotNull(video);
        }
    }
}