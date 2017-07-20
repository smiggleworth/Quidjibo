using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Quidjibo.Providers;

namespace Quidjibo.Aws.Sqs.Providers
{
    public class SqsQueueProvider : IQueueProvider
    {
        private readonly AmazonSQSClient _client;

        public bool SingleLoop => false;

        public SqsQueueProvider(AmazonSQSClient client)
        {
            _client = client;
        }

        public async Task CreateAsync(string name, CancellationToken cancellationToken)
        {
            var response = await _client.CreateQueueAsync(name, cancellationToken);
        }

        public Task GetAsync(string name, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}