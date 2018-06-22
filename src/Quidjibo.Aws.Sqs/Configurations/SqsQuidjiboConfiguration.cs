using Amazon.Runtime;
using Amazon.SQS;
using Quidjibo.Configurations;
using Quidjibo.Constants;

namespace Quidjibo.Aws.Sqs.Configurations
{
    public class SqsQuidjiboConfiguration : IQuidjiboConfiguration
    {
        /// <summary>
        ///     The credentials to connect to SQS.
        /// </summary>
        public AWSCredentials Credentials { get; set; }

        /// <summary>
        ///     The Amazon SQS configuration.
        /// </summary>
        public AmazonSQSConfig AmazonSqsConfig { get; set; }

        /// <summary>
        ///     The long poll duration to.
        /// </summary>
        public int LongPollDuration { get; set; } = 20;

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
        public int BatchSize { get; set; } = 10;

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