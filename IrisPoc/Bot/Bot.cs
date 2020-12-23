using System;
using System.Threading.Tasks;

namespace IrisPoc
{
    internal class Bot
    {
        private readonly IChatPollRulesConsumer _chatPollRulesConsumer;

        public Bot(IChatPollRulesConsumer chatPollRulesConsumer)
        {
            _chatPollRulesConsumer = chatPollRulesConsumer;
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

                    Request request = items[0].StartsWith("a")
                        ? Request.StartPoll
                        : Request.StopPoll;

                    var rule = new ChatPollRequest(
                        request,
                        new UserPollRule(
                            new User(
                                items[1],
                                items[2]),
                            TimeSpan.FromSeconds(int.Parse(items[3]))),
                        "chat");

                    Console.WriteLine($"Sending {rule}");

                    _chatPollRulesConsumer.Update(rule);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}