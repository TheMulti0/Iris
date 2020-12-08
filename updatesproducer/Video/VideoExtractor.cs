using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Common;

namespace UpdatesProducer
{
    public class VideoExtractor
    {
        private readonly VideoExtractorConfig _config;

        public VideoExtractor(VideoExtractorConfig config)
        {
            _config = config;
        }

        public async Task<Video> ExtractVideo(string url)
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

            TimeSpan? duration = GetDuration(durationSeconds);

            return new Video(
                extractedUrl,
                thumbnailUrl,
                IsBestFormat: true,
                duration,
                width,
                height);
        }

        private async Task<JsonElement> GetResponse(string url)
        {
            var response = await ScriptExecutor.ExecutePython(
                "extract_video.py",
                url,
                _config.FormatRequest,
                _config.UserName,
                _config.Password);

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