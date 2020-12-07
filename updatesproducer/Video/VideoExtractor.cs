using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Common;

namespace UpdatesProducer
{
    public static class VideoExtractor
    {
        public static async Task<Video> ExtractVideo(string url)
        {
            JsonElement root = await GetResponse(url);

            string highestFormatUrl = GetHighestFormatUrl(root);
            
            string extractedUrl = root.GetPropertyOrNull("url")?.GetString() ?? highestFormatUrl;

            if (extractedUrl == null)
            {
                throw new NullReferenceException("Extracted url was not found");
            }
            
            string thumbnailUrl = root.GetPropertyOrNull("thumbnail")?.GetString();
            double? durationSeconds = root.GetPropertyOrNull("duration")?.GetDoubleOrNull();
            int? width = root.GetPropertyOrNull("duration")?.GetIntOrNull();
            int? height = root.GetPropertyOrNull("duration")?.GetIntOrNull();

            // TimeSpan? duration = GetDuration(durationSeconds);

            return new Video
            {
                Url = extractedUrl,
                ThumbnailUrl = thumbnailUrl,
                DurationSeconds = (int?) durationSeconds,
                Width = width,
                Height = height,
                IsHighestFormatAvaliable = true
            };
        }

        private static async Task<JsonElement> GetResponse(string url)
        {
            var response = await ScriptExecutor.ExecutePython(
                "extract_video.py",
                url,
                "best");

            return JsonDocument.Parse(response).RootElement;
        }

        private static JsonElement.ArrayEnumerator? GetFormats(JsonElement root)
        {
            return root.GetPropertyOrNull("formats")?
                .EnumerateArray();
        }

        private static string GetHighestFormatUrl(JsonElement root)
        {
            return GetFormats(root)
                ?.LastOrDefault()
                .GetPropertyOrNull("url")
                ?.GetString();
        }

        private static TimeSpan? GetDuration(double? durationSeconds)
        {
            return durationSeconds != null
                ? TimeSpan.FromSeconds((double) durationSeconds)
                : null;
        }
    }
}