using Iris.Api;

namespace Updates.Facebook
{
    internal static class MediaFactory
    {
        public static Media ToMedia(string imageUrl)
        {
            return new Media(imageUrl, MediaType.Photo);
        }
    }
}