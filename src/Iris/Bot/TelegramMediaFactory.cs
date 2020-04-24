using System;
using Telegram.Bot.Types;
using Updates.Api;

namespace Iris.Bot
{
    internal static class TelegramMediaFactory
    {
        public static IAlbumInputMedia ToTelegramMedia(Media media)
        {
            var rawMedia = new InputMedia(media.Url);

            return media.Type switch
            {
                MediaType.Photo => new InputMediaPhoto(rawMedia),
                _ => new InputMediaVideo(rawMedia)
            };
        }
    }
}