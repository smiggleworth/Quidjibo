using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Quidjibo.Aws.Sqs.Configurations;
using Quidjibo.Aws.Sqs.Types;
using Quidjibo.Models;
using Quidjibo.Providers;

namespace Quidjibo.Aws.Sqs.Providers
{
    public class SqsWorkProvider : IWorkProvider
    {
        private const string WorkItemId = nameof(WorkItemId);
        private const string CorrelationId = nameof(CorrelationId);
        private const string Queue = "Queue";
        private readonly int _batchSize;

        private readonly AmazonSQSClient _client;
        private readonly string _queueUrl;
        private readonly SqsQueueType _type;
        private readonly int _visibilityTimeout;
        private readonly int _waitTimeSeconds;

        public SqsWorkProvider(AmazonSQSClient client, string queueUrl, SqsQueueType type, int visibilityTimeout, int batchSize, int waitTimeSeconds)
        {
            _client = client;
            _visibilityTimeout = visibilityTimeout;
            _batchSize = batchSize;
            _queueUrl = queueUrl;
            _type = type;
            if (waitTimeSeconds < 0 || waitTimeSeconds > 20)
            {
                throw new ArgumentException($"{nameof(waitTimeSeconds)} must be between 0 and 20.");
            }

            _waitTimeSeconds = waitTimeSeconds;
        }

        public async Task SendAsync(WorkItem item, int delay, CancellationToken cancellationToken)
        {
            var request = new SendMessageRequest(_queueUrl, Convert.ToBase64String(item.Payload));
            var id = item.Id.ToString();
            var correlationId = item.CorrelationId.ToString();
            if (_type == SqsQueueType.Fifo)
            {
                request.MessageDeduplicationId = id;
                request.MessageGroupId = id;
            }
            else
            {
                // per message delays only applies to standard
                request.DelaySeconds = delay;
            }

            request.MessageAttributes.Add(WorkItemId, new MessageAttributeValue
            {
                StringValue = id,
                DataType = "String"
            });
            request.MessageAttributes.Add(CorrelationId, new MessageAttributeValue
            {
                StringValue = correlationId,
                DataType = "String"
            });
            request.MessageAttributes.Add(Queue, new MessageAttributeValue
            {
                StringValue = item.Queue,
                DataType = "String"
            });

            await _client.SendMessageAsync(request, cancellationToken);
        }

        public async Task<List<WorkItem>> ReceiveAsync(string worker, CancellationToken cancellationToken)
        {
            var request = new ReceiveMessageRequest(_queueUrl)
            {
                MaxNumberOfMessages = _batchSize,
                VisibilityTimeout = _visibilityTimeout,
                WaitTimeSeconds = _waitTimeSeconds,
                MessageAttributeNames = new List<string> { WorkItemId, CorrelationId, Queue }
            };

            var messages = await _client.ReceiveMessageAsync(request, cancellationToken);
            return messages.Messages.Select(message => new WorkItem
            {
                Id = new Guid(message.MessageAttributes[WorkItemId].StringValue),
                CorrelationId = new Guid(message.MessageAttributes[CorrelationId].StringValue),
                Queue = message.MessageAttributes[Queue].StringValue,
                Token = message.ReceiptHandle,
                Payload = Convert.FromBase64String(message.Body)
            }).ToList();
        }

        public async Task<DateTime> RenewAsync(WorkItem item, CancellationToken cancellationToken)
        {
            var lockExpiration = (item.VisibleOn ?? DateTime.UtcNow).AddSeconds(_visibilityTimeout);
            await _client.ChangeMessageVisibilityAsync(_queueUrl, item.Token, _visibilityTimeout, cancellationToken);
            return lockExpiration;
        }

        public async Task CompleteAsync(WorkItem item, CancellationToken cancellationToken)
        {
            var request = new DeleteMessageRequest(_queueUrl, item.Token);
            await _client.DeleteMessageAsync(request, cancellationToken);
        }

        public async Task FaultAsync(WorkItem item, CancellationToken cancellationToken)
        {
            await _client.ChangeMessageVisibilityAsync(_queueUrl, item.Token, 0, cancellationToken);
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}