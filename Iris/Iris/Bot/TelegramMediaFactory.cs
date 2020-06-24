using Iris.Api;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace Iris.Bot
{
    internal static class TelegramMediaFactory
    {
        public static IAlbumInputMedia ToAlbumInputMedia(this Media media)
        {
            var rawMedia = new InputMedia(media.Url);

            return media.Type switch
            {
                MediaType.Photo => new InputMediaPhoto(rawMedia),
                _ => new InputMediaVideo(rawMedia)
            };
        }

        public static InputOnlineFile ToInputOnlineFile(this Media media)
        {
            return new InputOnlineFile(media.Url);
        }
    }
}