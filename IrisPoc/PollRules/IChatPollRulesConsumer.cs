namespace IrisPoc
{
    internal interface IChatPollRulesConsumer
    {
        void Update(ChatPollRequest request);
    }
}