using System;
using Common;
using Update = Common.Update;

namespace TelegramSender
{
    public class MessageInfoBuilder
    {
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
            
            return prefix + update.Content + suffix;
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
                    return $"<a href=\"{url}\">{text.Content}</a>";
                
                case TextMode.Text:
                    return text.Content;
                
                case TextMode.Url:
                    return url;
            }

            return string.Empty;
        }
    }
}