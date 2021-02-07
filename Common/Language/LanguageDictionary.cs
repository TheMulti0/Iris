using System;

namespace Common
{
    public class LanguageDictionary
    {
        public string LanguageString { get; set; }
        public string AddUser { get; set; }
        public string UsersFound { get; set; }
        public string NoUsersFound { get; set; }
        public string NoChatId { get; set; }
        public string NoChat { get; set; }
        public string ConnectedToChat { get; set; }
        public string NotConnected { get; set; }
        public string DisconnectedFrom { get; set; }
        public string Back { get; set; }
        public string SettingsFor { get; set; }
        public string UserId { get; set; }
        public string Platform { get; set; }
        public string DisplayName { get; set; }
        public string MaxDelay { get; set; }
        public string Language { get; set; }
        public string SetDisplayName { get; set; }
        public string Remove { get; set; }
        public string Removed { get; set; }
        public string SelectPlatform { get; set; }
        public string EnterNewDisplayName { get; set; }
        public string EnterUserFromPlatform { get; set; }
        public string Added { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Feeds { get; set; }
        public string Done { get; set; }
        public string ChooseLanguage { get; set; }
        public string Repost { get; set; }
        public string ShowPrefix { get; set; }
        public string NotAdmin { get; set; }
        public string Disable { get; set; }
        public string Enable { get; set; }
        public string Disabled { get; set; }
        public string Enabled { get; set; }
        public string SetLanguage { get; set; }
        public string ShowSuffix { get; set; }
        public string SendScreenshotOnly { get; set; }
        public string PrivateDm { get; set; }
        public string Subscriptions { get; set; }
        public string Live { get; set; }
        public string UserNotFound { get; set; }
        public string Start { get; set; }
        public string Accept { get; set; }
        public string Decline { get; set; }
        public string YouMustAgreeToTos { get; set; }
        public string Tos { get; set; }
        public string ThanksForCheckingOut { get; set; }
        public string ThanksNowYouCanUse { get; set; }

        public string GetPlatform(Platform platform)
        {
            string p = null;
            
            switch (platform)
            {
                case Common.Platform.Facebook:
                    p = Facebook;
                    break;
                case Common.Platform.Twitter:
                    p = Twitter;
                    break;
                case Common.Platform.Feeds:
                    p = Feeds;
                    break;
            }
            
            return p ?? Enum.GetName(platform);
        }
    }
}