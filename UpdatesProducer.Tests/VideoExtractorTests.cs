using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace UpdatesProducer.Tests
{
    interface ITest
    {
        [JsonProperty("dur_ation")]
        public long DurHi { get; set; }
    }

    public class C
    {
        public string FirstName { get; set; }
    }
    
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

        [TestMethod]
        public void Test()
        {
            var a = JsonSerializer.Deserialize<C>("{\"firstName\": \"hi\"}", new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            Console.WriteLine("tes");
        }
    }
}