using System.Collections.Generic;
using System.Threading.Tasks;
using Common;

namespace TelegramReceiver
{
    internal class UserValidator
    {
        private readonly Dictionary<string, IPlatformValidator> _validators;

        public UserValidator(
            FacebookValidator facebook,
            TwitterValidator twitter)
        {
            _validators = new Dictionary<string, IPlatformValidator>
            {
                {
                    "facebook",
                    facebook
                },
                {
                    "twitter",
                    twitter
                }
            };
        }
        
        public Task<string> ValidateAsync(string userId, string platform)
        {
            return _validators[platform].ValidateAsync(userId);
        }
    }

}