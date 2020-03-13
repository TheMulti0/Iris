using Telegram.Bot.Types;
using Updates.Api;

namespace Iris
{
    internal static class TelegramMediaFactory
    {
        public static IAlbumInputMedia ToTelegramMedia(Media media)
        {
            var rawMedia = new InputMedia(media.Url);

            return media.Type switch
            {
                MediaType.Photo => new InputMediaPhoto(rawMedia),
                
                MediaType.Video => new InputMediaVideo(rawMedia), 
                MediaType.AnimatedGif => new InputMediaVideo(rawMedia) 
            };
        }
    }
}