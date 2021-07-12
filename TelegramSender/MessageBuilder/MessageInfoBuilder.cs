using System.Text.RegularExpressions;
using Common;
using Update = Common.Update;

namespace TelegramSender
{
    public class MessageInfoBuilder
    {
        private const string TwitterUrl = "https://twitter.com";
        
        private const string TwitterUserNamePattern = @"@(?<userName>[\w\d-_]+)";
        private static readonly Regex TwitterUserNameRegex = new(TwitterUserNamePattern);
    
        public MessageInfo Build(Update update, UserChatSubscription chatSubscription)
        {
            string message = GetMessage(update, chatSubscription);

            if (chatSubscription.SendScreenshotOnly && update.Screenshot != null)
            {
                return new MessageInfo(
                    message,
                    new []{ new BytesPhoto(update.Screenshot) },
                    chatSubscription.ChatInfo.Id);
            }
            
            return new MessageInfo(
                message,
                update.Media,
                chatSubscription.ChatInfo.Id,
                DisableWebPagePreview: !chatSubscription.ShowUrlPreview);
        }

        private static string GetMessage(Update update, UserChatSubscription chatSubscription)
        {
            var prefix = ToString(chatSubscription.Prefix, update.Url);
            var suffix = ToString(chatSubscription.Suffix, update.Url);

            if (prefix != string.Empty)
            {
                prefix += "\n\n\n";
            }
            if (suffix != string.Empty)
            {
                suffix = $"\n\n\n{suffix}";
            }

            string updateContent = GetContent(update);
            return prefix + updateContent + suffix;
        }

        private static string GetContent(Update update)
        {
            if (update.Author.Platform != Platform.Twitter)
            {
                return update.Content;
            }
            
            return TwitterUserNameRegex.Replace(
                update.Content,
                m =>
                {
                    string username = m.Value;
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