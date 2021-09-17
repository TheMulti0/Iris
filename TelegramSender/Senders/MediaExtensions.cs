using System;
using System.Collections.Generic;
using System.Linq;
using Scraper.Net;
using TdLib;
using TelegramClient;

namespace TelegramSender
{
    public static class MediaExtensions
    {
        public static IEnumerable<TdApi.InputMessageContent> GetInputMessageContent(this MessageInfo message, TdApi.FormattedText text)
        {
            TdApi.FormattedText GetCaption(int i) => i == 0 && message.FitsInOneMediaMessage ? text : null;

            int mediaIndex = 0;
            
            foreach (IMediaItem mediaItem in message.MediaItems)
            {
                yield return mediaItem.ToInputMessageContent(GetCaption(mediaIndex));
                
                mediaIndex++;
            }
        }
        
        public static TdApi.InputMessageContent ToInputMessageContent(
            this IMediaItem media,
            TdApi.FormattedText caption)
        {
            switch (media)
            {
                case PhotoItem p:
                    return p.ToInputPhoto(caption);
                
                case VideoItem v:
                    return v.ToInputVideo(caption);
                
                case LocalVideoItem l:
                    return l.ToInputVideo(caption);
            }

            throw new ArgumentOutOfRangeException(media.GetType().FullName);
        }

        private static TdApi.InputMessageContent ToInputPhoto(this PhotoItem photo, TdApi.FormattedText caption)
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

        private static TdApi.InputMessageContent ToInputVideo(this VideoItem video, TdApi.FormattedText caption)
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

        private static TdApi.InputMessageContent ToInputVideo(this LocalVideoItem video, TdApi.FormattedText caption)
        {
            int height = video.Height ?? 0;
            int width = video.Width ?? 0;

            var remoteThumbnail = new TdApi.InputFile.InputFileRemote
            {
                Id = video.ThumbnailUrl
            };
            var localThumbnail = new InputRecyclingLocalFile(video.ThumbnailUrl);
            return new TdApi.InputMessageContent.InputMessageVideo
            {
                Caption = caption,
                Height = height,
                Width = width,
                Duration = (int) video.Duration.TotalSeconds,
                Video = new InputRecyclingLocalFile(video.Url),
                Thumbnail = new TdApi.InputThumbnail
                {
                    Height = height,
                    Width = width,
                    Thumbnail = video.IsThumbnailLocal 
                        ? localThumbnail 
                        : remoteThumbnail
                },
                SupportsStreaming = true
            };
        }

        public static TdApi.InputMessageContent ToInputMessageContent(this TdApi.MessageContent content)
        {
            switch (content)
            {
                case TdApi.MessageContent.MessagePhoto p:
                    return p.ToInputPhoto();
                
                case TdApi.MessageContent.MessageVideo v:
                    return v.ToInputVideo();
            }

            return null;
        }

        private static TdApi.InputMessageContent.InputMessagePhoto ToInputPhoto(
            this TdApi.MessageContent.MessagePhoto photo)
        {
            TdApi.PhotoSize size = photo.Photo.Sizes.OrderByDescending(size => size.Width).First();
            return new TdApi.InputMessageContent.InputMessagePhoto
            {
                //Caption = photo.Caption,
                Height = size.Height,
                Width = size.Width,
                Photo = size.Photo.ToInputFile()
            };
        }
        
        private static TdApi.InputMessageContent.InputMessageVideo ToInputVideo(
            this TdApi.MessageContent.MessageVideo video)
        {
            var v = video.Video;
            
            return new TdApi.InputMessageContent.InputMessageVideo
            {
                //Caption = video.Caption,
                Duration = v.Duration,
                Height = v.Height,
                Width = v.Width,
                Thumbnail = v.Thumbnail?.ToInputThumbnail(),
                Video = v.Video_.ToInputFile(),
                SupportsStreaming = v.SupportsStreaming
            };
        }
        
        private static TdApi.InputThumbnail ToInputThumbnail(
            this TdApi.PhotoSize thumbnail)
        {
            return new()
            {
                Thumbnail = thumbnail.Photo.ToInputFile(),
                Height = thumbnail.Height,
                Width = thumbnail.Width
            };
        }

        private static TdApi.InputFile ToInputFile(this TdApi.File file)
        {
            return new TdApi.InputFile.InputFileRemote
            {
                Id = file.Remote.Id
            };
        }
    }
}