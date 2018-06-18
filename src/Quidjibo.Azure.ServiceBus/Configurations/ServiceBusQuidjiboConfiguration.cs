using Quidjibo.Configurations;
using Quidjibo.Constants;

namespace Quidjibo.Azure.ServiceBus.Configurations
{
    public class ServiceBusQuidjiboConfiguration : IQuidjiboConfiguration
    {
        public string[] Queues { get; } = Default.Queues;
        public bool SingleLoop { get; } = false;
        public int MaxAttempts { get; } = 10;
        public int LockInterval { get; } = 30;
        public int Throttle { get; } = 5;
        public bool EnableWorker { get; set; } = true;
        public bool EnableScheduler { get; set; }
    }
}