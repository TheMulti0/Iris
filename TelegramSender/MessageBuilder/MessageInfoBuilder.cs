using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Common;
using Scraper.Net;
using Scraper.Net.Screenshot;
using Scraper.RabbitMq.Common;

namespace TelegramSender
{
    public class MessageInfoBuilder
    {
        private const string TwitterUrl = "https://twitter.com";
        
        private const string TwitterUserNamePattern = @"@(?<userName>[\w\d-_]+)";
        private static readonly Regex TwitterUserNameRegex = new(TwitterUserNamePattern);
    
        public MessageInfo Build(NewPost newPost, UserChatSubscription chatSubscription)
        {
            string message = GetMessage(newPost, chatSubscription);

            IEnumerable<IMediaItem> screenshots = newPost.Post.MediaItems
                .Where(item => item is ScreenshotItem)
                .ToList();
            if (chatSubscription.SendScreenshotOnly && screenshots.Any())
            {
                return new MessageInfo(
                    message,
                    screenshots,
                    chatSubscription.ChatInfo.Id);
            }
            
            return new MessageInfo(
                message,
                newPost.Post.MediaItems,
                chatSubscription.ChatInfo.Id,
                DisableWebPagePreview: !chatSubscription.ShowUrlPreview);
        }

        private static string GetMessage(NewPost newPost, UserChatSubscription chatSubscription)
        {
            var prefix = ToString(chatSubscription.Prefix, newPost.Post.Url);
            var suffix = ToString(chatSubscription.Suffix, newPost.Post.Url);

            if (prefix != string.Empty)
            {
                prefix += "\n\n\n";
            }
            if (suffix != string.Empty)
            {
                suffix = $"\n\n\n{suffix}";
            }

            string updateContent = GetContent(newPost);
            return prefix + updateContent + suffix;
        }

        private static string GetContent(NewPost newPost)
        {
            if (newPost.Platform != "twitter")
            {
                return newPost.Post.Content;
            }
            
            return TwitterUserNameRegex.Replace(
                newPost.Post.Content,
                m =>
                {
                    string username = m.Groups["userName"].Value;
                    return HyperlinkText(username, $"{TwitterUrl}/{username}");
                });
        }

        private static string ToString(Text text, string url)
        {
            if (!text.Enabled)
            {
                return string.Empty;
            }

            switch (text.Mode)
            {
                case TextMode.HyperlinkedText:
                    return HyperlinkText(text.Content, url);
                
                case TextMode.Text:
                    return text.Content;
                
                case TextMode.Url:
                    return url;
            }

            return string.Empty;
        }

        private static string HyperlinkText(string text, string url) => $"<a href=\"{url}\">{text}</a>";
    }
}