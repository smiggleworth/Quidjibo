using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Quidjibo.Models;
using Quidjibo.Providers;

namespace Quidjibo.Aws.Sqs.Providers
{
    public class SqsWorkProvider : IWorkProvider
    {
        private const string WorkItemId = nameof(WorkItemId);
        private const string CorrelationId = nameof(CorrelationId);
        private readonly int _batchSize;

        private readonly AmazonSQSClient _client;
        private readonly string _queueUrl;
        private readonly int _visibilityTimeout;

        public SqsWorkProvider(AmazonSQSClient client, string queueUrl, int visibilityTimeout, int batchSize)
        {
            _client = client;
            _visibilityTimeout = visibilityTimeout;
            _batchSize = batchSize;
            _queueUrl = queueUrl;
        }

        public async Task SendAsync(WorkItem item, int delay, CancellationToken cancellationToken)
        {
            var request = new SendMessageRequest(_queueUrl, Convert.ToBase64String(item.Payload));
            var id = item.Id.ToString();
            var correlationId = item.CorrelationId.ToString();
            request.MessageDeduplicationId = id;
            request.DelaySeconds = delay;

            request.MessageAttributes.Add(WorkItemId, new MessageAttributeValue
            {
                StringValue = id
            });
            request.MessageAttributes.Add(CorrelationId, new MessageAttributeValue
            {
                StringValue = correlationId
            });

            var response = await _client.SendMessageAsync(request, cancellationToken);

            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
            }
        }

        public async Task<List<WorkItem>> ReceiveAsync(string worker, CancellationToken cancellationToken)
        {
            var request = new ReceiveMessageRequest(_queueUrl)
            {
                MaxNumberOfMessages = _batchSize,
                VisibilityTimeout = _visibilityTimeout,
                WaitTimeSeconds = int.MaxValue
            };

            var messages = await _client.ReceiveMessageAsync(request, cancellationToken);
            return messages.Messages.Select(message => new WorkItem
                           {
                               Id = new Guid(message.MessageAttributes[WorkItemId].StringValue),
                               CorrelationId = new Guid(message.MessageAttributes[WorkItemId].StringValue),
                               Token = message.ReceiptHandle
                           })
                           .Cast<WorkItem>()
                           .ToList();
        }

        public async Task<DateTime> RenewAsync(WorkItem workItem, CancellationToken cancellationToken)
        {
            var lockExpiration = (workItem.VisibleOn ?? DateTime.UtcNow).AddSeconds(_visibilityTimeout);
            var response = await _client.ChangeMessageVisibilityAsync(_queueUrl, workItem.Token, _visibilityTimeout,
                cancellationToken);

            // todo handle errors

            return lockExpiration;
        }

        public async Task CompleteAsync(WorkItem workItem, CancellationToken cancellationToken)
        {
            var request = new DeleteMessageRequest(_queueUrl, workItem.Token);
            var response = await _client.DeleteMessageAsync(request, cancellationToken);
        }

        public async Task FaultAsync(WorkItem workItem, CancellationToken cancellationToken)
        {
            var response = await _client.ChangeMessageVisibilityAsync(_queueUrl, workItem.Token, 0, cancellationToken);
        }
    }
}