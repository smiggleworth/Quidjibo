using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Quidjibo.Aws.Sqs.Configurations;
using Quidjibo.Aws.Sqs.Providers;
using Quidjibo.Factories;
using Quidjibo.Providers;

namespace Quidjibo.Aws.Sqs.Factories
{
    public class SqsWorkProviderFactory : IWorkProviderFactory
    {
        private readonly MemoryCache _providerCache = new MemoryCache(new MemoryCacheOptions());
        private readonly ILoggerFactory _loggerFactory;
        private readonly SqsQuidjiboConfiguration _sqsQuidjiboConfiguration;
        private readonly AmazonSQSClient _client;

        public SqsWorkProviderFactory(
            ILoggerFactory loggerFactory,
            SqsQuidjiboConfiguration sqsQuidjiboConfiguration)
        {
            _loggerFactory = loggerFactory;
            _sqsQuidjiboConfiguration = sqsQuidjiboConfiguration;
            _client = new AmazonSQSClient(_sqsQuidjiboConfiguration.Credentials, _sqsQuidjiboConfiguration.AmazonSqsConfig);
        }

        public Task<IWorkProvider> CreateAsync(string queues, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _providerCache.GetOrCreateAsync(queues, async e =>
            {
                var response = await _client.GetQueueUrlAsync(queues, cancellationToken);
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    throw new InvalidOperationException("Could not load the queues url.");
                }

                var provider = new SqsWorkProvider(
                    _loggerFactory.CreateLogger<SqsWorkProvider>(),
                    _client,
                    response.QueueUrl,
                    _sqsQuidjiboConfiguration.Type,
                    _sqsQuidjiboConfiguration.LockInterval,
                    _sqsQuidjiboConfiguration.BatchSize,
                    _sqsQuidjiboConfiguration.LongPollDuration);
                return (IWorkProvider)provider;
            });
        }

        public Task<IWorkProvider> CreateAsync(string[] queues, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (queues.Length != 1)
            {
                throw new NotSupportedException("Each queues requires a separate listener. Please pass a single queue.");
            }

            return CreateAsync(queues[0], cancellationToken);
        }
    }
}