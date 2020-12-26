using System;

namespace Common
{
    public record UserPollRule(
        User User,
        TimeSpan? Interval);
}