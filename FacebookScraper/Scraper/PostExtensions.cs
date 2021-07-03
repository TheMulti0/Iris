using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;

namespace FacebookScraper
{
    internal static class PostExtensions
    {
        private const string LinkRegex = "\n(?<link>[A-Z].+)";

        public static Post ToPost(this PostRaw raw)
        {
            return new()
            {
                Id = raw.PostId,
                EntireText = raw.GetCleanText(),
                PostTextOnly = raw.PostText,
                CreationDate = raw.Time,
                Url = raw.PostUrl,
                Author = new Author
                {
                    Id = raw.UserId,
                    Url = raw.UserUrl,
                    UserName = raw.UserName
                },
                IsLive = raw.IsLive,
                Link = raw.Link,
                Images = raw.GetImages().ToArray(),
                Video = raw.GetVideo(),
                Stats = new Stats
                {
                    Comments = raw.Comments,
                    Shares = raw.Shares,
                    Likes = raw.Likes
                },
                SharedPost = raw.GetSharedPost(),
                Comments = raw.CommentsFull
            };
        }
        
        private static string GetCleanText(this PostRaw raw)
        {
            if (raw.Link != null)
            {
                return raw.Text.Replace(
                    new[]
                    {
                        LinkRegex
                    },
                    raw.Link);
            }

            return raw.Text;
        }

        private static IEnumerable<Image> GetImages(this PostRaw raw)
        {
            return raw.ImageIds.Select((id, index) => new Image
            {
                Id = id,
                Url = raw.Images.ElementAt(index),
                LowQualityUrl = raw.ImagesLowQuality.ElementAtOrDefault(index),
                Description = raw.ImagesDescription.ElementAtOrDefault(index)
            });
        }

        private static Video GetVideo(this PostRaw raw)
        {
            if (raw.VideoId == null)
            {
                return null;
            }   
            
            return new Video
            {
                Id = raw.VideoId,
                Url = raw.VideoUrl,
                Width = raw.VideoWidth,
                Height = raw.VideoHeight,
                Quality = raw.VideoQuality,
                ThumbnailUrl = raw.VideoThumbnail,
                SizeMb = raw.VideoSizeMb,
                Watches = raw.VideoWatches
            };
        }

        private static SharedPost GetSharedPost(this PostRaw raw)
        {
            if (raw.SharedPostId == null)
            {
                return null;
            }
            
            return new SharedPost
            {
                Id = raw.SharedPostId,
                Url = raw.SharedPostUrl,
                Text = raw.SharedText,
                CreationDate = raw.SharedTime,
                Author = new Author
                {
                    Id = raw.SharedUserId,
                    UserName = raw.SharedUserName
                }
            };
        }
    }
}