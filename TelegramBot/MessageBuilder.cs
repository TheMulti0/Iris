using System.Collections.Generic;
using System.Linq;
using Common;
using Telegram.Bot.Types;
using Update = Common.Update;

namespace TelegramBot
{
    public class MessageBuilder
    {
        private readonly IEnumerable<FilterRule> _filterRules;

        public MessageBuilder(TelegramConfig config)
        {
            _filterRules = config.FilterRules ?? new List<FilterRule>();
        }

        public MessageInfo Build(Update update, string source, User user, ChatId chatId)
        {
            bool FindFilterRule(FilterRule rule)
            {
                if (!user.UserNames.Any(userName => rule.UserNames.Contains(userName)))
                {
                    // Rule doesn't affect this user
                    return false;
                }
                
                // Rule affects this user and is set for every chatid (because ChatIds is empty)
                // Or rule affects this user and is set for this chatid (because ChatIds is specific)
                return !rule.ChatIds.Any() ||
                       rule.ChatIds.Any(c => c == chatId);
            }

            FilterRule filterRule = _filterRules.FirstOrDefault(FindFilterRule);

            if (filterRule?.SkipReposts == true && update.Repost)
            {
                throw new FilterRuleSkipException();
            }

            string message;
            if (filterRule?.HideMessagePrefix == true)
            {
                message = update.Content;
            }
            else
            {
                string repostPrefix = update.Repost ? " בפרסום מחדש" : string.Empty;

                message = $"<a href=\"{update.Url}\">{user.DisplayName}{repostPrefix} ({source}):</a>\n\n\n{update.Content}";
            }

            List<IMedia> media = filterRule?.DisableMedia == true
                ? new List<IMedia>()
                : update.Media;
            
            return new MessageInfo(
                message,
                media,
                chatId);
        }
    }
}