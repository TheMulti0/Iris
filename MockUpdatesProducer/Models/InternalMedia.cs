using System.Text.Json.Serialization;
using UpdatesConsumer;

namespace MockUpdatesProducer
{
    internal class InternalMedia : Audio
    {
        [JsonPropertyName("_type")]
        public string _type { get; set; }
    }
}