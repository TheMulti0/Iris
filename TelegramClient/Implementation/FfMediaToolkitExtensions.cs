using System;
using System.IO;
using System.Threading.Tasks;
using FFMediaToolkit.Decoding;
using FFMediaToolkit.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using TdLib;
using Size = System.Drawing.Size;

namespace TelegramClient
{
    public static class FfMediaToolkitExtensions
    {
        public static async Task<TdApi.InputMessageContent.InputMessageVideo> ExtractInfoAsync(this TdApi.InputMessageContent.InputMessageVideo v)
        {
            if (v.Video is not TdApi.InputFile.InputFileLocal l)
            {
                return null;
            }

            var video = MediaFile.Open(l.Path).Video;
            Size size = video.Info.FrameSize;

            TdApi.InputThumbnail thumbnail = v.Thumbnail;
            if (v.Thumbnail == null)
            {
                thumbnail = await video.ExtractThumbnailAsync();
            }

            return new TdApi.InputMessageContent.InputMessageVideo
            {
                Duration = (int) video.Info.Duration.TotalSeconds,
                Height = size.Height,
                Thumbnail = thumbnail,
                Width = size.Width
            };
        }

        private static async Task<TdApi.InputThumbnail> ExtractThumbnailAsync(this VideoStream video)
        {
            TimeSpan duration = video.Info.Duration;
            Size size = video.Info.FrameSize;

            var inputStream = new MemoryStream();
            
            await video.GetFrame(duration / 2)
                .ToBitmap()
                .SaveAsync(inputStream, new PngEncoder());
            
            inputStream.Seek(0, SeekOrigin.Begin);

            return new TdApi.InputThumbnail
            {
                Height = size.Height,
                Width = size.Width,
                Thumbnail = new InputFileStream(async () => inputStream)
            };
        }

        private static Image<Bgr24> ToBitmap(this ImageData imageData)
        {
            return Image.LoadPixelData<Bgr24>(imageData.Data, imageData.ImageSize.Width, imageData.ImageSize.Height);
        }
    }
}