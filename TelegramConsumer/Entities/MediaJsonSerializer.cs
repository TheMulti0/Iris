using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TelegramConsumer
{
    public class MediaJsonSerializer : JsonConverter<IMedia>
    {
        private const string TypeDiscriminator = "_type";

        public override IMedia Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            JsonElement rootElement = document.RootElement;
            string rawText = rootElement.GetRawText();

            string type = rootElement.GetProperty(TypeDiscriminator).GetString();

            switch (type)
            {
                default:
                    return JsonSerializer.Deserialize<Photo>(rawText, options);
                case "Video":
                    return JsonSerializer.Deserialize<Video>(rawText, options);
                case "Audio":
                    return JsonSerializer.Deserialize<Audio>(rawText, options);
            }
        }

        public override void Write(Utf8JsonWriter writer, IMedia value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}