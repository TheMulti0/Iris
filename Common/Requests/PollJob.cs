using System;

namespace Common
{
    public record PollJob(
        User User,
        DateTime? MinimumEarliestUpdateTime);
}