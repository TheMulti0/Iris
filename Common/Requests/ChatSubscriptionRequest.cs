namespace Common
{
    public record ChatSubscriptionRequest(
        SubscriptionType SubscriptionType,
        Subscription Subscription,
        long ChatId) : SubscriptionRequest(SubscriptionType, Subscription);
}