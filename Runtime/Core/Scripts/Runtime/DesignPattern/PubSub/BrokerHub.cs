namespace NIX.Core.DesignPatterns
{
    public static class BrokerHub
    {
        public static readonly MessageBroker Global = new();
        public static readonly TopicMessageBroker GlobalTopic = new();
    }
}