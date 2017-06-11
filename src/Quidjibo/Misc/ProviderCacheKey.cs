namespace Quidjibo.Misc
{
    public struct ProviderCacheKey<TKey>
        where TKey : IQuidjiboClientKey
    {
        public string QueueName { get; }

        public ProviderCacheKey(string queueName)
        {
            QueueName = queueName;
        }
    }
}