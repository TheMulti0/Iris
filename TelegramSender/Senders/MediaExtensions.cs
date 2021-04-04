using System.Collections.Generic;
using Common;
using TdLib;

namespace TelegramSender
{
    internal static class MediaExtensions
    {
        public static IEnumerable<TdApi.InputMessageContent> GetInputMedia(this MessageInfo message, TdApi.FormattedText text)
        {
            TdApi.FormattedText GetCaption(int i) => i == 0 && message.FitsInOneMediaMessage ? text : null;

            int mediaIndex = 0;
            
            foreach (IMedia media in message.Media)
            {
                yield return media.ToInputMessageContent(GetCaption(mediaIndex));
                
                mediaIndex++;
            }
        }
        
        public static TdApi.InputMessageContent ToInputMessageContent(this IMedia media, TdApi.FormattedText caption)
        {
            switch (media)
            {
                case Photo p:
                    return p.ToInputPhoto(caption);
                
                case Video v:
                    return v.ToInputVideo(caption);
            }

            return null;
        }

        private static TdApi.InputMessageContent ToInputPhoto(this Photo photo, TdApi.FormattedText caption)
        {
            return new TdApi.InputMessageContent.InputMessagePhoto
            {
                Caption = caption,
                Photo = new TdApi.InputFile.InputFileRemote
                {
                    Id = photo.Url
                }
            };
        }

        private static TdApi.InputMessageContent ToInputVideo(this Video video, TdApi.FormattedText caption)
        {
            int height = video.Height ?? 0;
            int width = video.Width ?? 0;

            return new TdApi.InputMessageContent.InputMessageVideo
            {
                Caption = caption,
                Height = height,
                Width = width,
                Duration = (int) (video.Duration?.TotalSeconds ?? 0),
                Video = new TdApi.InputFile.InputFileRemote
                {
                    Id = video.Url
                },
                Thumbnail = new TdApi.InputThumbnail
                {
                    Height = height,
                    Width = width,
                    Thumbnail = new TdApi.InputFile.InputFileRemote
                    {
                        Id = video.ThumbnailUrl
                    }
                },
                SupportsStreaming = true
            };
        }
    }
}