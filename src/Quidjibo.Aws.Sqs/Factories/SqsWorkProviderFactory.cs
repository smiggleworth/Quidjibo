using System;
using System.Collections.Generic;
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
        private readonly BasicAWSCredentials _basicAwsCredentials;


        public async Task<IWorkProvider> CreateAsync(string queue,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var client = new AmazonSQSClient(_basicAwsCredentials, _amazonSqsConfig);
            var response = await client.GetQueueUrlAsync(queue, cancellationToken);
            if (response.HttpStatusCode != HttpStatusCode.OK)
            {
                throw new InvalidOperationException("Could not load the queue url.");
            }

            var provider = new SqsWorkProvider(client, response.QueueUrl, 30, 10);
            return provider;
        }

        public SqsWorkProviderFactory(
            BasicAWSCredentials basicAwsCredentials,
            AmazonSQSConfig amazonSqsConfig)
        {
            _basicAwsCredentials = basicAwsCredentials;
            _amazonSqsConfig = amazonSqsConfig;
        }

        public Task<IWorkProvider> CreateAsync(List<string> queues,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotSupportedException("Each queue requires a seperate listener.");
        }
    }
}