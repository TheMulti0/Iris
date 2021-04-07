using System.Collections.Generic;
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
        public static async IAsyncEnumerable<TdApi.InputMessageContent> GetInputMessageContentAsync(this MessageInfo message, TdApi.FormattedText text)
        {
            TdApi.FormattedText GetCaption(int i) => i == 0 && message.FitsInOneMediaMessage ? text : null;

            int mediaIndex = 0;
            
            foreach (IMedia media in message.Media)
            {
                yield return await media.ToInputMessageContentAsync(GetCaption(mediaIndex));
                
                mediaIndex++;
            }
        }
        
        public static async Task<TdApi.InputMessageContent> ToInputMessageContentAsync(
            this IMedia media,
            TdApi.FormattedText caption)
        {
            switch (media)
            {
                case Photo p:
                    return p.ToInputPhoto(caption);
                
                case BytesPhoto p:
                    return await p.ToInputPhotoAsync(caption);

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
        
        private static async Task<TdApi.InputMessageContent> ToInputPhotoAsync(
            this BytesPhoto photo,
            TdApi.FormattedText caption)
        {
            Task<Stream> GetStreamAsync() => Task.FromResult<Stream>(new MemoryStream(photo.Bytes));

            var inputFileStream = new InputFileStream(GetStreamAsync);

            var inputMessageContent = new TdApi.InputMessageContent.InputMessagePhoto
            {
                Caption = caption,
                Photo = await inputFileStream.GetFileAsync()
            };
            
            return new InputMessageContentFileStream
            {
                InputFileStream = inputFileStream,
                InputMessageContent = inputMessageContent
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
            return new()
            {
                //Caption = video.Caption,
                Duration = video.Video.Duration,
                Height = video.Video.Height,
                Width = video.Video.Width,
                Thumbnail = new TdApi.InputThumbnail
                {
                    Thumbnail = video.Video.Thumbnail.Photo.ToInputFile(),
                    Height = video.Video.Thumbnail.Height,
                    Width = video.Video.Thumbnail.Width
                },
                Video = video.Video.Video_.ToInputFile(),
                SupportsStreaming = video.Video.SupportsStreaming
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