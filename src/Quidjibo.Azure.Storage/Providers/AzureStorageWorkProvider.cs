using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Quidjibo.Models;
using Quidjibo.Providers;

namespace Quidjibo.Azure.Storage.Providers
{
    public class AzureStorageWorkProvider : IWorkProvider
    {
        private readonly CloudQueue _cloudQueue;
        private readonly int _batchSize;
        private readonly int _visibilityTimeout;

        public AzureStorageWorkProvider(CloudQueue cloudQueue, int visibilityTimeout, int batchSize)
        {
            _cloudQueue = cloudQueue;
            _batchSize = batchSize;
            _visibilityTimeout = visibilityTimeout;
        }

        public Task SendAsync(WorkItem item, int delay, CancellationToken cancellationToken)
        {
            var message = new CloudQueueMessage(item.Id.ToString(), item.CorrelationId.ToString());
            message.SetMessageContent(item.Payload);
            var timeToLive = item.ExpireOn - DateTime.UtcNow;
            return _cloudQueue.AddMessageAsync(message, timeToLive, TimeSpan.FromSeconds(delay), null, null, cancellationToken);
        }

        public async Task<List<WorkItem>> ReceiveAsync(string worker, CancellationToken cancellationToken)
        {
            var messages = await _cloudQueue.GetMessagesAsync(_batchSize);
            return messages.Select(m => new WorkItem
            {
                Attempts = m.DequeueCount,
                CorrelationId = new Guid(m.PopReceipt),
                ExpireOn = m.ExpirationTime?.DateTime ?? DateTime.UtcNow,
                Id = new Guid(m.Id),
                Payload = m.AsBytes,
                VisibleOn = m.NextVisibleTime?.DateTime,
                RawMessage = m
            }).ToList();
        }

        public async Task<DateTime> RenewAsync(WorkItem workItem, CancellationToken cancellationToken)
        {
            var message = (CloudQueueMessage)workItem.RawMessage;
            var visibilityTimeout = TimeSpan.FromSeconds(_visibilityTimeout);
            var visibleOn = DateTime.UtcNow.Add(visibilityTimeout);
            await _cloudQueue.UpdateMessageAsync(message, visibilityTimeout, MessageUpdateFields.Visibility);
            return visibleOn;
        }

        public Task CompleteAsync(WorkItem workItem, CancellationToken cancellationToken)
        {
            var message = (CloudQueueMessage)workItem.RawMessage;
            return _cloudQueue.DeleteMessageAsync(message);
        }

        public Task FaultAsync(WorkItem workItem, CancellationToken cancellationToken)
        {
            var message = (CloudQueueMessage)workItem.RawMessage;
            return _cloudQueue.UpdateMessageAsync(message, TimeSpan.Zero, MessageUpdateFields.Visibility);
        }

        public void Dispose()
        {
        }
    }
}