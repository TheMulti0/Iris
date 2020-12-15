using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Common;

namespace UpdatesProducer
{
    public class VideoExtractor
    {
        private class VideoInfo
        {
            public string ExtractedUrl { get; set; }
            public string ThumbnailUrl { get; set; }
            public double? DurationSeconds { get; set; }
            public int? Width { get; set; }
            public int? Height { get; set; }
        }
        
        private readonly VideoExtractorConfig _config;

        public VideoExtractor(VideoExtractorConfig config)
        {
            _config = config;
        }

        public async Task<Video> ExtractVideo(string url)
        {
            JsonElement root = await GetResponse(url);

            JsonElement? highestFormat = GetFormats(root)?.LastOrDefault();
            
            var videoInfo = GetCombinedVideoInfo(root, highestFormat);

            if (videoInfo.ExtractedUrl == null)
            {
                throw new NullReferenceException("Extracted url was not found");
            }

            TimeSpan? duration = GetDuration(videoInfo.DurationSeconds);

            return new Video(
                videoInfo.ExtractedUrl,
                videoInfo.ThumbnailUrl,
                duration,
                videoInfo.Width,
                videoInfo.Height);
        }

        private static VideoInfo GetCombinedVideoInfo(JsonElement? root, JsonElement? highestFormat)
        {
            VideoInfo videoInfo = GetVideoInfo(root);
            VideoInfo fallbackInfo = GetVideoInfo(highestFormat);

            CombineVideoInfos(fallbackInfo, videoInfo);

            return videoInfo;
        }

        private static void CombineVideoInfos(VideoInfo fallbackInfo, VideoInfo videoInfo)
        {
            foreach (PropertyInfo property in typeof(VideoInfo).GetProperties())
            {
                var fallbackValue = property.GetValue(fallbackInfo);

                if (property.GetValue(videoInfo) == null && fallbackValue != null)
                {
                    property.SetValue(videoInfo, fallbackValue!);
                }
            }
        }

        private static VideoInfo GetVideoInfo(JsonElement? element)
        {
            return new()
            {
                ExtractedUrl = element?.GetPropertyOrNull("url")?.GetString(),
                ThumbnailUrl = element?.GetPropertyOrNull("thumbnail")?.GetString(),
                DurationSeconds = element?.GetPropertyOrNull("duration")?.GetDoubleOrNull(),
                Width = element?.GetPropertyOrNull("width")?.GetIntOrNull(),
                Height = element?.GetPropertyOrNull("height")?.GetIntOrNull()
            };
        }

        private async Task<JsonElement> GetResponse(string url)
        {
            var output
                = await ScriptExecutor.ExecutePython(
                "extract_video.py",
                url,
                _config.FormatRequest,
                _config.UserName,
                _config.Password);

            // Cut out the json element, ignore the logs and other outputs
            string response = output.Substring(
                output.IndexOf('{'),
                output.Length);

            return JsonDocument.Parse(response).RootElement;
        }

        private static JsonElement.ArrayEnumerator? GetFormats(JsonElement root)
        {
            return root.GetPropertyOrNull("formats")?
                .EnumerateArray();
        }

        private static TimeSpan? GetDuration(double? durationSeconds)
        {
            return durationSeconds != null
                ? TimeSpan.FromSeconds((double) durationSeconds)
                : null;
        }
    }
}