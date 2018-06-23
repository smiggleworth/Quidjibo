using Quidjibo.Configurations;
using Quidjibo.Constants;

namespace Quidjibo.Azure.ServiceBus.Configurations
{
    public class ServiceBusQuidjiboConfiguration : IQuidjiboConfiguration
    {
        public string[] Queues { get; set; } = Default.Queues;
        public bool SingleLoop { get; } = false;
        public int MaxAttempts { get; set; } = 10;
        public int BatchSize { get; set; } = 10;
        public int LockInterval { get; set; } = 30;
        public int Throttle { get; set; } = 5;
        public bool EnableWorker { get; set; } = true;
        public int? WorkPollingInterval { get; set; }
        public bool EnableScheduler { get; set; }
        public int? SchedulePollingInterval { get; set; }
    }
}