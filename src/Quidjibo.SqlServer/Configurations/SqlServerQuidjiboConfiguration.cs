using System.Collections.Generic;
using Quidjibo.Configurations;

namespace Quidjibo.SqlServer.Configurations
{
    public class SqlServerQuidjiboConfiguration : IQuidjiboConfiguration
    {
        public bool EnableScheduler { get; set; } = true;
        public bool EnableWorker { get; set; } = true;
        public bool SingleLoop { get; set; } = true;
        public int LockInterval { get; set; } = 30;
        public int MaxAttempts { get; set; } = 5;
        public int Throttle { get; set; } = 10;
        public string ConnectionString { get; set; }
        public string[] Queues { get; set; }
    }
}