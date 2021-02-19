using System;

namespace Common
{
    public record PollJob(
        User User,
        DateTime? MinimumEarliestUpdateTime)
    {
        public override string ToString()
        {
            return MinimumEarliestUpdateTime == null
                ? $"{{ PollJob: {User} }}"
                : $"{{ PollJob: {User}, From: {MinimumEarliestUpdateTime} }}";
        }
    }
}