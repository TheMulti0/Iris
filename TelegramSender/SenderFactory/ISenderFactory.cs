using System.Threading.Tasks;

namespace TelegramSender
{
    public interface ISenderFactory
    {
        Task<MessageSender> CreateAsync();
    }
}