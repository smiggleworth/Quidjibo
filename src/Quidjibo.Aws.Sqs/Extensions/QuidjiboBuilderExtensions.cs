using System;
using Quidjibo.Aws.Sqs.Configurations;
using Quidjibo.Aws.Sqs.Factories;

namespace Quidjibo.Aws.Sqs.Extensions
{
    public static class QuidjiboBuilderExtensions
    {
        /// <summary>
        ///     Use SQS for the queue infrastructure
        /// </summary>
        /// <param name="builder">The QuidjiboBuilder.</param>
        /// <param name="sqsQuidjiboConfiguration">The configuration.</param>
        /// <returns></returns>
        public static QuidjiboBuilder UseSqs(this QuidjiboBuilder builder, SqsQuidjiboConfiguration sqsQuidjiboConfiguration)
        {
            return builder.Configure(sqsQuidjiboConfiguration)
                          .ConfigureWorkProviderFactory(new SqsWorkProviderFactory(builder.LoggerFactory, sqsQuidjiboConfiguration));
        }

        /// <summary>
        ///     Use SQS for the queue infrastructure
        /// </summary>
        /// <param name="builder">The QuidjiboBuilder.</param>
        /// <param name="sqsQuidjiboConfiguration">The configuration.</param>
        /// <returns></returns>
        public static QuidjiboBuilder UseSqs(this QuidjiboBuilder builder, Action<SqsQuidjiboConfiguration> sqsQuidjiboConfiguration)
        {
            var config = new SqsQuidjiboConfiguration();
            sqsQuidjiboConfiguration(config);
            return builder.UseSqs(config);
        }
    }
}