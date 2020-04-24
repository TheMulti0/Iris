namespace Updates.Api
{
    public class Media
    {
        public string Url { get; }

        public MediaType Type { get; }
        
        public Media(string url, MediaType type)
        {
            Url = url;
            Type = type;
        }
    }
}