using Quidjibo.Configurations;

namespace Quidjibo.Azure.Storage.Configurations
{
    public class AzureStorageQuidjiboConfiguration : IQuidjiboConfiguration
    {
        public string ConnectionString { get; set; }
        public string[] Queues { get; }
        public bool SingleLoop { get; }
        public int MaxAttempts { get; }
        public int BatchSize { get; }
        public int LockInterval { get; }
        public int Throttle { get; }
        public bool EnableWorker { get; set; }
        public int? WorkPollingInterval { get; set; }
        public bool EnableScheduler { get; set; }
        public int? SchedulePollingInterval { get; set; }
    }
}