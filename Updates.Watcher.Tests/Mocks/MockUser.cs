using Updates.Api;

namespace Updates.Watcher.Tests
{
    internal class MockUser : IUser
    {
        public long Id { get; }
        
        public string Name { get; }
        
        public string DisplayName { get; }
        
        public string Url { get; }
        
        public MockUser(
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