using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace TelegramConsumer
{
    internal static class TelegramMediaExtensions
    {
        public static InputOnlineFile ToInputOnlineFile(this Media media)
        {
            return new InputOnlineFile(media.Url);
        }

        public static IAlbumInputMedia ToAlbumInputMedia(this Media media)
        {
            var inputMedia = new InputMedia(media.Url);

            return media.Type switch
            {
                MediaType.Photo => new InputMediaPhoto(inputMedia),
                _ => new InputMediaVideo(inputMedia)
            };
        }
    }
}