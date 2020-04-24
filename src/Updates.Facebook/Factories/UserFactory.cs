using System.Linq;
using Updates.Api;

namespace Updates.Facebook
{
    internal static class UserFactory
    {
        public static User ToUser(string userTokens)
        {
            string[] tokens = userTokens.Split(':');
            
            return new User(
                tokens.FirstOrDefault(),
                tokens[1],
                tokens[1],
                null);
        }
    }
}