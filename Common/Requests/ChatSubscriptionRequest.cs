namespace Common
{
    public record ChatSubscriptionRequest(
        SubscriptionType SubscriptionType,
        Subscription Subscription,
        string ChatId) : SubscriptionRequest(SubscriptionType, Subscription);
}