namespace IrisPoc
{
    internal record ChatPollRequest(
        Request Request,
        UserPollRule PollRule,
        string ChatId);
}