using Quidjibo.Configurations;

namespace Quidjibo.Azure.ServiceBus.Configurations
{
    public class ServiceBusQuidjiboConfiguration : IQuidjiboConfiguration
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