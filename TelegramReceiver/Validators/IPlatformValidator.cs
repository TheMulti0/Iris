using System.Threading.Tasks;
using Common;

namespace TelegramReceiver
{
    internal interface IPlatformValidator
    {
        Task<User> ValidateAsync(string userId);
    }
}