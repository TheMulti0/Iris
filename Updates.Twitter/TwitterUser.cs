using Updates.Api;

namespace Updates.Twitter
{
    public class TwitterUser : IUser
    {
        public long Id { get; }
        
        public string Name { get; }
        
        public string DisplayName { get; }
        
        public string Url { get; }

        public TwitterUser(Tweetinvi.Models.IUser user)
        {
            Id = user.Id;
            Name = user.ScreenName;
            DisplayName = user.Name;
            Url = user.Url;
        }
        
        public bool Equals(IUser other) => Id == other?.Id;
    }
}