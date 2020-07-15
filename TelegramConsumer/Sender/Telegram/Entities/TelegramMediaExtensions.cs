using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
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
            return media.ToInputMedia();
        }
        
        public static IAlbumInputMedia ToAlbumInputMedia(
            this Media media,
            string caption,
            ParseMode parseMode)
        {
            dynamic albumInputMedia = media.ToInputMedia();
            
            // Set InputMediaBase properties
            albumInputMedia.Caption = caption;
            albumInputMedia.ParseMode = parseMode;
            
            return albumInputMedia;
        }

        private static dynamic ToInputMedia(this Media media)
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