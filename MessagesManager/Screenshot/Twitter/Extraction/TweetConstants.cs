namespace MessagesManager
{
    public static class TweetConstants
    {
        public const string TweetXPath = "/html/body/div/div/div/div[2]/main/div/div/div/div[1]/div/div[2]/div/section/div/div/div[1]/div/div/article";
        public static readonly string TweetImagesXPath = $"{TweetXPath}/div/div/div/div[3]/div[2]";
        public static readonly string TweetDateXPath = $"{TweetXPath}/div/div/div/div[3]/div[3]";
        public static readonly string TweetStatsXPath = $"{TweetXPath}/div/div/div/div[3]/div[4]";
        public static readonly string TweetButtonsXPath = $"{TweetXPath}/div/div/div/div[3]/div[5]";

        public static string GetReplyTweetXPath(int tweetIndex)
            => $"/html/body/div/div/div/div[2]/main/div/div/div/div/div/div[2]/div/section/div/div/div[{tweetIndex}]/div/div/article";
        public static string GetReplyTweetTextXPath(string replyTweetXPath) => $"{replyTweetXPath}/div/div/div/div[3]/div[2]";
        public static string GetReplyTweetImagesXPath(string replyTweetXPath) => $"{replyTweetXPath}/div/div/div/div[3]/div[3]";
        public static string GetReplyTweetDateXPath(string replyTweetXPath) => $"{replyTweetXPath}/div/div/div/div[3]/div[4]";
        public static string GetReplyTweetStatsXPath(string replyTweetXPath) => $"{replyTweetXPath}/div/div/div/div[3]/div[5]";
        public static string GetReplyTweetButtonsXPath(string replyTweetXPath) => $"{replyTweetXPath}/div/div/div/div[3]/div[6]";
    }
}