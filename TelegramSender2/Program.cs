using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TelegramSender2
{
    public record InputMediaPhoto
    {
        [JsonPropertyName("media")]
        public string Media { get; init; }
        [JsonPropertyName("caption")]
        public string Caption { get; init; }
        [JsonPropertyName("parse_mode")]
        public string ParseMode { get; init; }
    }
    
    public record InputMediaVideo
    {
        [JsonPropertyName("media")]
        public string Media { get; init; }
        [JsonPropertyName("thumb")]
        public string Thumbnail { get; init; }
        [JsonPropertyName("caption")]
        public string Caption { get; init; }
        [JsonPropertyName("width")]
        public int Width { get; init; }
        [JsonPropertyName("height")]
        public int Height { get; init; }
        [JsonPropertyName("duration")]
        public int Duration { get; init; }
        [JsonPropertyName("supports_streaming")]
        public bool SupportsStreaming { get; init; }
        [JsonPropertyName("parse_mode")]
        public string ParseMode { get; init; }
    }

    public record SendMediaGroupRequest
    {
        [JsonPropertyName("chat_id")]
        public string ChatId { get; init; }
        [JsonPropertyName("photos")]
        public List<InputMediaPhoto> Photos { get; init; }
        [JsonPropertyName("videos")]
        public List<InputMediaVideo> Videos { get; init; }
        [JsonPropertyName("disable_notification")]
        public bool DisableNotification { get; init; }
        [JsonPropertyName("reply_to_message_id")]
        public int ReplyToMessageId { get; init; }
        [JsonPropertyName("schedule_date")]
        public int ScheduleDate { get; init; }
    }

    class Program
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        static async Task Main(string[] args)
        {
            var request = new SendMediaGroupRequest
            {
                ChatId = "-461267748",
                Photos = new List<InputMediaPhoto>
                {
                    new InputMediaPhoto
                    {
                        Caption = "Test",
                        Media =
                            "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fwww.nature-isere.fr%2Fsites%2Fdefault%2Ffiles%2Fimages%2Fespece%2Fpratique%2Fimage_par_thomas_compigne_de_pixabay.jpg&f=1&nofb=1"
                    }
                }
            };
            
            var requestJson = JsonSerializer.Serialize(request);
            
            var a = await HttpClient.GetStringAsync($"http://localhost:5000/send_media_group?request={requestJson}");
            Console.WriteLine(a);
        }
    }
}