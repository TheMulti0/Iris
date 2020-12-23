namespace IrisPoc
{
    internal static class Program
    {
        private static void Main()
        {
            ISetPollRulesConsumer setPollRulesConsumer = Orchestrator.WireUp();
            new Bot(setPollRulesConsumer).Run();
        }
    }
}