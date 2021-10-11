using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common;
using Scraper.MassTransit.Common;
using Scraper.Net;
using Scraper.Net.Screenshot;

namespace TelegramSender
{
    public class MessageInfoBuilder
    {
        public MessageInfo Build(NewPost newPost, UserChatSubscription chatSubscription, CancellationToken ct)
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
                    chatSubscription.ChatInfo.Id,
                    ct);
            }
            
            return new MessageInfo(
                message,
                newPost.Post.MediaItems,
                chatSubscription.ChatInfo.Id,
                ct,
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
            Post post = newPost.Post;

            string content = GetContentWithSuppliedHyperlinks(post);
            PostAuthor originalAuthor = post.OriginalAuthor;
            
            switch (newPost.Platform)
            {
                case "facebook" when originalAuthor != null:
                {
                    string name = originalAuthor.DisplayName;

                    return content
                        .ReplaceFirst(name, HyperlinkText(name, originalAuthor.Url));
                }

                default:
                    return content;
            }
        }

        private static string GetContentWithSuppliedHyperlinks(Post post)
        {
            return post.Hyperlinks.Aggregate(
                post.Content,
                (content, hyperlink) => content.Replace(hyperlink.Text, hyperlink.Url));
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