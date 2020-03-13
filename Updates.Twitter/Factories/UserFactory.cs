using Tweetinvi.Models;
using Updates.Api;

namespace Updates.Twitter
{
    internal static class UserFactory
    {
        public static User ToUser(IUser user)
        {
            long id = user.Id;
            string name = user.ScreenName;
            string displayName = user.Name;
            string url = user.Url;
            
            return new User(
                id,
                name,
                displayName,
                url);
        }
    }
}