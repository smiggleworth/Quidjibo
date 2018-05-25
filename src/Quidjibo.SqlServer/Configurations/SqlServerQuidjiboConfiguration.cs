using Quidjibo.Configurations;

namespace Quidjibo.SqlServer.Configurations
{
    public class SqlServerQuidjiboConfiguration : IQuidjiboConfiguration
    {
        /// <summary>
        ///     The ConnectionString to the Sql Server
        /// </summary>
        public string ConnectionString { get; set; }

        /// <inheritdoc />
        public string[] Queues { get; set; } = { "default" };

        /// <inheritdoc />
        public bool SingleLoop { get; set; } = true;

        /// <inheritdoc />
        public int MaxAttempts { get; set; } = 5;

        /// <inheritdoc />
        public int LockInterval { get; set; } = 30;

        /// <inheritdoc />
        public int Throttle { get; set; } = 10;

        /// <inheritdoc />
        public bool EnableWorker { get; set; } = true;

        /// <inheritdoc />
        public bool EnableScheduler { get; set; } = true;
    }
}