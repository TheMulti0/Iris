namespace IrisPoc
{
    internal static class Program
    {
        private static void Main()
        {
            IChatPollRulesConsumer chatPollRulesConsumer = Orchestrator.WireUp();
            new Bot(chatPollRulesConsumer).Run();
        }
    }
}