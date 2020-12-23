namespace IrisPoc
{
    internal record SetPollRule(
        PollRuleType Type,
        UserPollRule PollRule,
        string ChatId);
}