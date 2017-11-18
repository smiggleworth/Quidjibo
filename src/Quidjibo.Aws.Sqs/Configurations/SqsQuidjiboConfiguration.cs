using System.Collections.Generic;
using Quidjibo.Configurations;

namespace Quidjibo.Aws.Sqs.Configurations
{
    public class SqsQuidjiboConfiguration : IQuidjiboConfiguration
    {
        public List<string> PublisherQueues { get; set; }
        public List<string> Queues { get; set; }

        public bool SingleLoop => false;


        public int PollingInterval => 0;
        public int MaxAttempts { get; set; }
        public int LockInterval => 30;
        public int Throttle { get; set; }
        public bool EnableWorker { get; set; }
        public bool EnableScheduler { get; set; }
    }
}