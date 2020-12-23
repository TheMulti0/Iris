using System;
using System.Threading.Tasks;

namespace IrisPoc
{
    internal class Bot
    {
        private readonly ISetPollRulesConsumer _setPollRulesConsumer;

        public Bot(ISetPollRulesConsumer setPollRulesConsumer)
        {
            _setPollRulesConsumer = setPollRulesConsumer;
        }

        public void Run()
        {
            while (true)
            {
                Console.WriteLine("Add or Delete - UserId - Source - Interval Seconds");

                string requestString = Console.ReadLine();

                Console.WriteLine("\n\n");

                try
                {
                    string[] items = requestString.Split(' ');

                    PollRuleType pollRuleType = items[0].StartsWith("a")
                        ? PollRuleType.Poll
                        : PollRuleType.StopPoll;

                    var request = new SetPollRule(
                        pollRuleType,
                        new UserPollRule(
                            new User(
                                items[1],
                                items[2]),
                            TimeSpan.FromSeconds(int.Parse(items[3]))),
                        "chat");

                    Console.WriteLine($"Sending {request}");

                    _setPollRulesConsumer.Update(request);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}