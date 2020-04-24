namespace Updates.Api
{
    public class User
    {
        public string Id { get; }

        public string Name { get; }

        public string DisplayName { get; }

        public string Url { get; }
        
        public User(
            string id,
            string name,
            string displayName,
            string url)
        {
            Id = id;
            Name = name;
            DisplayName = displayName;
            Url = url;
        }
    }
}