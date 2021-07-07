using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Common;

namespace MessagesManager
{
    public class VideoExtractor
    {
        private const string ScriptName = "extract_video.py";
        private readonly VideoExtractorConfig _config;

        public VideoExtractor(VideoExtractorConfig config)
        {
            _config = config;
        }

        public async Task<Video> ExtractVideo(string url)
        {
            JsonElement root = await GetVideoInfoAsync(url);

            JsonElement? highestFormat = GetFormats(root)?.LastOrDefault();
            
            VideoInfo videoInfo = GetCombinedVideoInfo(root, highestFormat);

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

        private async Task<JsonElement> GetVideoInfoAsync(string url)
        {
            ExtractVideoResponse response = await GetResponse(url);

            HandleError(response);

            return (JsonElement) response.VideoInfo;
        }

        private async Task<ExtractVideoResponse> GetResponse(string url)
        {
            var request = new ExtractVideoRequest
            {
                Url = url,
                Format = _config.FormatRequest,
                UserName = _config.UserName,
                Password = _config.Password
            };
            
            string json = JsonSerializer.Serialize(request)
                .Replace("\"", "\\\""); // Python argument's double quoted strings need to be escaped
            
            string responseString = await GetResponseString(json);

            var response = JsonSerializer.Deserialize<ExtractVideoResponse>(responseString);

            return response with { OriginalRequest = request };
        }

        private static async Task<string> GetResponseString(string json)
        {
            string output
                = await ScriptExecutor.ExecutePython(
                    ScriptName,
                    token: default,
                    json);

            // Cut out the json element, ignore the logs and other outputs
            int startIndex = output.IndexOf('{');
            int lastIndex = output.LastIndexOf('}') + 1;

            return output.Substring(
                startIndex,
                lastIndex - startIndex);
        }

        private static void HandleError(ExtractVideoResponse response)
        {
            switch (response.Error)
            {
                default:
                    if (response.VideoInfo == null)
                    {
                        throw new Exception($"Unrecognized error {response.Error} {response.ErrorDescription}");    
                    }
                    break;
            }
        }

        private static JsonElement.ArrayEnumerator? GetFormats(JsonElement root)
        {
            return root.GetPropertyOrNull("formats")?.EnumerateArray();
        }

        private static VideoInfo GetCombinedVideoInfo(JsonElement? root, JsonElement? highestFormat)
        {
            VideoInfo videoInfo = GetVideoInfo(root);
            VideoInfo fallbackInfo = GetVideoInfo(highestFormat);

            CombineVideoInfos(fallbackInfo, videoInfo);

            return videoInfo;
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

        private static TimeSpan? GetDuration(double? durationSeconds)
        {
            return durationSeconds != null
                ? TimeSpan.FromSeconds((double) durationSeconds)
                : null;
        }
    }
}