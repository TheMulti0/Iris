using System.Text.Json.Serialization;

namespace Iris.Api
{
    public class User
    {
        public string Id { get; set; }

        public string Name { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Gender Gender { get; set; }
    }
}