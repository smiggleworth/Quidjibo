using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Microsoft.Extensions.Logging;
using Quidjibo.Providers;

namespace Quidjibo.Aws.Sqs.Providers
{
    public class SqsQueueProvider : IQueueProvider
    {
        private readonly ILogger _logger;
        private readonly AmazonSQSClient _client;

        public SqsQueueProvider(
            ILogger logger,
            AmazonSQSClient client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task CreateAsync(string name, CancellationToken cancellationToken)
        {
            var response = await _client.CreateQueueAsync(name, cancellationToken);
            _logger.LogInformation("The request to create queue {0} return status code : {1}", name, response.HttpStatusCode);
        }

        public Task GetAsync(string name, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}