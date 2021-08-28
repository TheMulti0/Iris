using System.Threading.Tasks;
using Common;

namespace TelegramReceiver
{
    internal interface IPlatformValidator
    {
        Task<string> ValidateAsync(string userId);
    }
}