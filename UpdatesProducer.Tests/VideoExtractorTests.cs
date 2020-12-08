using System.Threading.Tasks;
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

            var videoExtractor = new VideoExtractor(new VideoExtractorConfig
            {
                FormatRequest = "best"
            });
            var video = await videoExtractor.ExtractVideo(url);
            
            Assert.IsNotNull(video);
        }
    }
}