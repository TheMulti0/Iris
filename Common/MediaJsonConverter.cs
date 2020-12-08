using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common
{
    public class MediaJsonConverter : JsonConverter<IMedia>
    {
        private const string TypeDiscriminator = "type";

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
                
                case nameof(Video):
                    return JsonSerializer.Deserialize<Video>(rawText, options);
                
                case nameof(Audio):
                    return JsonSerializer.Deserialize<Audio>(rawText, options);
            }
        }

        public override void Write(
            Utf8JsonWriter writer, IMedia value, JsonSerializerOptions options)
        {
            Type valueType = value.GetType();
            
            var dictionary = new Dictionary<string, object>
            {
                {
                    TypeDiscriminator, valueType.Name
                }
            };

            foreach (PropertyInfo property in valueType.GetProperties())
            {
                dictionary.Add(property.Name, property.GetValue(value));
            }
            
            JsonSerializer.Serialize(writer, dictionary);
        }
    }
}