using System.Collections.Generic;
using Quidjibo.Configurations;

namespace Quidjibo.SqlServer.Configurations
{
    public class SqlServerQuidjiboConfiguration : IQuidjiboConfiguration
    {
        public List<string> PublisherQueues { get; set; }

        public SqlServerQuidjiboConfiguration()
        {
            Throttle = 10;
            LockInterval = 30;
            MaxAttempts = 5;
            SingleLoop = true;
        }

        public List<string> Queues { get; set; }
        public bool SingleLoop { get; set; }
        public int PollingInterval { get; set; }
        public int MaxAttempts { get; set; }
        public int LockInterval { get; set; }
        public int Throttle { get; set; }
    }
}