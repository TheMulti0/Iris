namespace TelegramBot
{
    public class FilterRule
    {
        public string[] UserNames { get; set; } = new string[0];
        
        public string[] ChatIds { get; set; } = new string[0];

        public bool HideMessagePrefix { get; set; } = false;

        public bool DisableMedia { get; set; } = false;
    }
}