using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.SQS;
using Quidjibo.Aws.Sqs.Providers;
using Quidjibo.Factories;
using Quidjibo.Providers;

namespace Quidjibo.Aws.Sqs.Factories
{
    public class SqsWorkProviderFactory : IWorkProviderFactory
    {
        private readonly AmazonSQSConfig _amazonSqsConfig;
        private readonly AWSCredentials _awsCredentials;
        private readonly int _visibilityTimeout;
        private readonly int _longPollDuration;

        public SqsWorkProviderFactory(
            AWSCredentials awsCredentials,
            AmazonSQSConfig amazonSqsConfig, 
            int? visibilityTimeout = 60, 
            int? longPollDuration = 0)
        {
            _awsCredentials = awsCredentials;
            _amazonSqsConfig = amazonSqsConfig;
            _visibilityTimeout = visibilityTimeout ?? 60;
            _longPollDuration = longPollDuration ?? 0;
        }
        
        public int PollingInterval => 10;

        public async Task<IWorkProvider> CreateAsync(string queues,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var client = new AmazonSQSClient(_awsCredentials, _amazonSqsConfig);
            var response = await client.GetQueueUrlAsync(queues, cancellationToken);
            if(response.HttpStatusCode != HttpStatusCode.OK)
            {
                throw new InvalidOperationException("Could not load the queues url.");
            }

            var provider = new SqsWorkProvider(client, response.QueueUrl, _visibilityTimeout, 10, _longPollDuration);
            return provider;
        }

        public Task<IWorkProvider> CreateAsync(string[] queues,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotSupportedException("Each queues requires a seperate listener.");
        }
    }
}