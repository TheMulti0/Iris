﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common;
using TdLib;
using TelegramClient;

namespace TelegramSender
{
    internal static class MediaExtensions
    {
        public static IEnumerable<TdApi.InputMessageContent> GetInputMessageContentAsync(this MessageInfo message, TdApi.FormattedText text)
        {
            TdApi.FormattedText GetCaption(int i) => i == 0 && message.FitsInOneMediaMessage ? text : null;

            int mediaIndex = 0;
            
            foreach (IMedia media in message.Media)
            {
                yield return media.ToInputMessageContentAsync(GetCaption(mediaIndex));
                
                mediaIndex++;
            }
        }
        
        public static TdApi.InputMessageContent ToInputMessageContentAsync(
            this IMedia media,
            TdApi.FormattedText caption)
        {
            switch (media)
            {
                case Photo p:
                    return p.ToInputPhoto(caption);
                
                case BytesPhoto p:
                    return p.ToInputPhotoAsync(caption);

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
        
        private static TdApi.InputMessageContent ToInputPhotoAsync(
            this BytesPhoto photo,
            TdApi.FormattedText caption)
        {
            Task<Stream> GetStreamAsync() => Task.FromResult<Stream>(new MemoryStream(photo.Bytes));

            var inputFileStream = new InputRemoteStream(GetStreamAsync);

            return new TdApi.InputMessageContent.InputMessagePhoto
            {
                Caption = caption,
                Photo = inputFileStream
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

        public static TdApi.InputMessageContent ToInputMessageContentAsync(this TdApi.MessageContent content)
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