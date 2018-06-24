using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Quidjibo.Aws.Sqs.Configurations;
using Quidjibo.Aws.Sqs.Providers;
using Quidjibo.Factories;
using Quidjibo.Providers;

namespace Quidjibo.Aws.Sqs.Factories
{
    public class SqsWorkProviderFactory : IWorkProviderFactory
    {
        private readonly SqsQuidjiboConfiguration _sqsQuidjiboConfiguration;

        public SqsWorkProviderFactory(SqsQuidjiboConfiguration sqsQuidjiboConfiguration)
        {
            _sqsQuidjiboConfiguration = sqsQuidjiboConfiguration;
        }

        public async Task<IWorkProvider> CreateAsync(string queues, CancellationToken cancellationToken = default(CancellationToken))
        {
            var client = new AmazonSQSClient(_sqsQuidjiboConfiguration.Credentials, _sqsQuidjiboConfiguration.AmazonSqsConfig);
            var response = await client.GetQueueUrlAsync(queues, cancellationToken);
            if (response.HttpStatusCode != HttpStatusCode.OK)
            {
                throw new InvalidOperationException("Could not load the queues url.");
            }

            var provider = new SqsWorkProvider(
                client,
                response.QueueUrl,
                _sqsQuidjiboConfiguration.Type,
                _sqsQuidjiboConfiguration.LockInterval,
                _sqsQuidjiboConfiguration.BatchSize,
                _sqsQuidjiboConfiguration.LongPollDuration);

            return provider;
        }

        public Task<IWorkProvider> CreateAsync(string[] queues, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (queues.Length != 1)
            {
                throw new NotSupportedException("Each queues requires a seperate listener. Please pass a single queue.");
            }

            return CreateAsync(queues[0], cancellationToken);
        }
    }
}