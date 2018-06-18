using Amazon.Runtime;
using Amazon.SQS;
using Quidjibo.Configurations;

namespace Quidjibo.Aws.Sqs.Configurations
{
    public class SqsQuidjiboConfiguration : IQuidjiboConfiguration
    {
        /// <summary>
        ///     The credentials to connect to SQS
        /// </summary>
        public AWSCredentials Credentials { get; set; }

        /// <summary>
        ///     The Amazon SQS configuration
        /// </summary>
        public AmazonSQSConfig AmazonSqsConfig { get; set; }

        /// <summary>
        ///     The long poll duration
        /// </summary>
        public int LongPollDuration { get; set; } = 0;

        public string[] Queues { get; set; }
        public bool SingleLoop => false;
        public int MaxAttempts { get; set; }
        public int LockInterval => 30;
        public int Throttle { get; set; }
        public bool EnableWorker { get; set; }
        public bool EnableScheduler { get; set; }
    }
}