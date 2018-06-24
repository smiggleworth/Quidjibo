

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using Quidjibo.Aws.Sqs.Types;
using Quidjibo.Models;
using Quidjibo.Providers;

namespace Quidjibo.Aws.Sqs.Providers
{
    public class SqsWorkProvider : IWorkProvider
    {
        private readonly int _batchSize;
        private readonly ILogger _logger;
        private readonly AmazonSQSClient _client;

        private readonly List<string> _messageAttributeNames = new List<string>
        {
            SqsMessageAttributes.CorrelationId,
            SqsMessageAttributes.Name,
            SqsMessageAttributes.Queue,
            SqsMessageAttributes.ScheduleId,
            SqsMessageAttributes.WorkItemId
        };

        private readonly string _queueUrl;
        private readonly SqsQueueType _type;
        private readonly int _visibilityTimeout;
        private readonly int _waitTimeSeconds;

        public SqsWorkProvider(
            ILogger logger,
            AmazonSQSClient client, 
            string queueUrl, 
            SqsQueueType type, 
            int visibilityTimeout, 
            int batchSize, 
            int waitTimeSeconds)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

        /// <inheritdoc />
        public async Task SendAsync(WorkItem item, int delay, CancellationToken cancellationToken)
        {
            var request = new SendMessageRequest(_queueUrl, Convert.ToBase64String(item.Payload));
            var id = item.Id.ToString();
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

            request.MessageAttributes.Add(SqsMessageAttributes.WorkItemId, new MessageAttributeValue
            {
                StringValue = id,
                DataType = "String"
            });
            request.MessageAttributes.Add(SqsMessageAttributes.CorrelationId, new MessageAttributeValue
            {
                StringValue = item.CorrelationId.ToString(),
                DataType = "String"
            });
            request.MessageAttributes.Add(SqsMessageAttributes.Name, new MessageAttributeValue
            {
                StringValue = item.Name,
                DataType = "String"
            });
            request.MessageAttributes.Add(SqsMessageAttributes.Queue, new MessageAttributeValue
            {
                StringValue = item.Queue,
                DataType = "String"
            });
            request.MessageAttributes.Add(SqsMessageAttributes.ScheduleId, new MessageAttributeValue
            {
                StringValue = (item.ScheduleId ?? Guid.Empty).ToString(),
                DataType = "String"
            });
            await _client.SendMessageAsync(request, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<List<WorkItem>> ReceiveAsync(string worker, CancellationToken cancellationToken)
        {
            var request = new ReceiveMessageRequest(_queueUrl)
            {
                MaxNumberOfMessages = _batchSize,
                VisibilityTimeout = _visibilityTimeout,
                WaitTimeSeconds = _waitTimeSeconds,
                MessageAttributeNames = _messageAttributeNames
            };
            var messages = await _client.ReceiveMessageAsync(request, cancellationToken);
            return messages.Messages.Select(message => new WorkItem
            {
                CorrelationId = new Guid(message.MessageAttributes[SqsMessageAttributes.CorrelationId].StringValue),
                Id = new Guid(message.MessageAttributes[SqsMessageAttributes.WorkItemId].StringValue),
                Name = message.MessageAttributes[SqsMessageAttributes.Name].StringValue,
                Payload = Convert.FromBase64String(message.Body),
                Queue = message.MessageAttributes[SqsMessageAttributes.Queue].StringValue,
                ScheduleId = new Guid(message.MessageAttributes[SqsMessageAttributes.ScheduleId].StringValue),
                Token = message.ReceiptHandle
            }).ToList();
        }

        /// <inheritdoc />
        public async Task<DateTime> RenewAsync(WorkItem item, CancellationToken cancellationToken)
        {
            var lockExpiration = (item.VisibleOn ?? DateTime.UtcNow).AddSeconds(_visibilityTimeout);
            await _client.ChangeMessageVisibilityAsync(_queueUrl, item.Token, _visibilityTimeout, cancellationToken);
            return lockExpiration;
        }

        /// <inheritdoc />
        public async Task CompleteAsync(WorkItem item, CancellationToken cancellationToken)
        {
            var request = new DeleteMessageRequest(_queueUrl, item.Token);
            await _client.DeleteMessageAsync(request, cancellationToken);
        }

        /// <inheritdoc />
        public async Task FaultAsync(WorkItem item, CancellationToken cancellationToken)
        {
            await _client.ChangeMessageVisibilityAsync(_queueUrl, item.Token, 0, cancellationToken);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}