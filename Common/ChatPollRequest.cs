namespace Common
{
    public record ChatPollRequest(
        Request Request,
        UserPollRule PollRule,
        string ChatId) : PollRequest(Request, PollRule);
}