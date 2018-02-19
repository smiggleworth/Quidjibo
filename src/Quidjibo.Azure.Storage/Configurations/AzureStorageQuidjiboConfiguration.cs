using Quidjibo.Configurations;

namespace Quidjibo.Azure.Storage.Configurations
{
    public class AzureStorageQuidjiboConfiguration : IQuidjiboConfiguration
    {
        public int PollingInterval { get; }
        public string[] Queues { get; }
        public bool SingleLoop { get; }
        public int MaxAttempts { get; }
        public int LockInterval { get; }
        public int Throttle { get; }
        public bool EnableWorker { get; set; }
        public bool EnableScheduler { get; set; }
    }
}