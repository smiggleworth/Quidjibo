using System;
using System.Linq;
using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.SQS;
using Amazon.Util;
using Amazon.Util.Internal.PlatformServices;
using Quidjibo.Aws.Sqs.Configurations;
using Quidjibo.Aws.Sqs.Factories;

namespace Quidjibo.Aws.Sqs.Extensions
{
    public static class QuidjiboBuilderExtensions
    {
        /// <summary>
        /// Use Sqs for Work Jobs only
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="config"></param>
        /// <param name="amazonSqsConfig"></param>
        /// <param name="awsCredentials"></param>
        /// <param name="visibilityTimeout"></param>
        /// <param name="longPollDuration"></param>
        /// <returns></returns>
        public static QuidjiboBuilder UseSqs(this QuidjiboBuilder builder, SqsQuidjiboConfiguration config = null, 
            AmazonSQSConfig amazonSqsConfig = null, AWSCredentials awsCredentials = null, 
            int? visibilityTimeout = null, int? longPollDuration = null)
        {
            amazonSqsConfig = amazonSqsConfig ?? new AmazonSQSConfig();
            
            if (awsCredentials == null)
            {
                new CredentialProfileStoreChain().TryGetAWSCredentials("default", out awsCredentials);
            }
            if (awsCredentials == null && string.IsNullOrEmpty(EC2InstanceMetadata.InstanceId))
            {
                var firstRole = InstanceProfileAWSCredentials.GetAvailableRoles().FirstOrDefault();
                if(firstRole != null)
                {
                    awsCredentials = new InstanceProfileAWSCredentials(firstRole);
                }
            }
            if(awsCredentials == null)
            {
                throw new ArgumentException($"{nameof(awsCredentials)} could not be loaded from the ~/.aws/credentials or the instance profile. Please pass explicitly.");
            }

            return builder.ConfigureIfSet(config)
                          .ConfigureWorkProviderFactory(new SqsWorkProviderFactory(awsCredentials, amazonSqsConfig, visibilityTimeout, longPollDuration))
                          .ConfigureProgressProviderFactory(new SqsProgressProviderFactory())
                          .ConfigureScheduleProviderFactory(new SqsScheduleProviderFactory());
        }

        private static QuidjiboBuilder ConfigureIfSet(this QuidjiboBuilder builder, SqsQuidjiboConfiguration config)
        {
            if(config != null)
            {
                builder.Configure(config);
            }
            return builder;
        }
    }
}
