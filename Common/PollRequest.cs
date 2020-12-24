namespace Common
{
    public record PollRequest(
        Request Request,
        UserPollRule PollRule);
}