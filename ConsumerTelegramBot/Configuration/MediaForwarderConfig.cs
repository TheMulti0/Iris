using System.Text.Json;
using System.Threading.Tasks;

namespace ConsumerTelegramBot.Configuration
{
    internal class MediaForwarderConfig
    {
        public long[] PostChannelIds { get; set; }

        public string Token { get; set; }
        
        public TwitterConfig TwitterConfig { get; set; }
    }
}