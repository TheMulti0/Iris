using System.Collections.Generic;
using System.Threading.Tasks;
using Common;

namespace TelegramReceiver
{
    internal class UserValidator
    {
        private readonly Dictionary<Platform, IPlatformValidator> _validators;

        public UserValidator(
            FacebookValidator facebook,
            TwitterValidator twitter)
        {
            _validators = new Dictionary<Platform, IPlatformValidator>
            {
                {
                    Platform.Facebook,
                    facebook
                },
                {
                    Platform.Twitter,
                    twitter
                }
            };
        }
        
        public Task<User> ValidateAsync(User request)
        {
            return _validators[request.Platform].ValidateAsync(request);
        }
    }

}