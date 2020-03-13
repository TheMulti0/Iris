namespace Updates.Api
{
    public class User
    {
        public long Id { get; }

        public string Name { get; }

        public string DisplayName { get; }

        public string Url { get; }
        
        public User(
            long id,
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