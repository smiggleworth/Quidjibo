using Quidjibo.Configurations;
using Quidjibo.Constants;

namespace Quidjibo.SqlServer.Configurations
{
    public class SqlServerQuidjiboConfiguration : IQuidjiboConfiguration
    {
        public int PollingInterval { get; set; } = 10;
        public string ConnectionString { get; set; }
        public int BatchSize { get; set; } = 5;

        /// <inheritdoc />
        public int? WorkPollingInterval { get; set; }

        /// <inheritdoc />
        public bool EnableScheduler { get; set; } = true;

        /// <inheritdoc />
        public int? SchedulePollingInterval { get; set; }

        /// <inheritdoc />
        public bool EnableWorker { get; set; } = true;

        /// <inheritdoc />
        public bool SingleLoop { get; set; } = true;

        /// <inheritdoc />
        public int LockInterval { get; set; } = 30;

        /// <inheritdoc />
        public int MaxAttempts { get; set; } = 5;

        /// <inheritdoc />
        public int Throttle { get; set; } = 10;

        /// <inheritdoc />
        public string[] Queues { get; set; } = Default.Queues;
    }
}