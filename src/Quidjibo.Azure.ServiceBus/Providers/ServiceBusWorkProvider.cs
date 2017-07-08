using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Quidjibo.Models;
using Quidjibo.Providers;


namespace Quidjibo.Azure.ServiceBus.Providers
{
    public class ServiceBusWorkProvider : IWorkProvider
    {
        private readonly MessageSender _sender;
        private readonly MessageReceiver _receiver;
        private readonly int _batchSize;

        public ServiceBusWorkProvider(MessageSender sender, MessageReceiver receiver, int batchSize)
        {
            _sender = sender;
            _receiver = receiver;
            _batchSize = batchSize;
        }

        public Task SendAsync(WorkItem item, int delay, CancellationToken cancellationToken)
        {
            var message = new Message(item.Payload)
            {
                MessageId = item.Id.ToString(),
                TimeToLive = item.ExpireOn - DateTime.UtcNow,
                CorrelationId = item.CorrelationId.ToString(),
                ScheduledEnqueueTimeUtc = DateTime.UtcNow.AddSeconds(delay)
            };
            message.UserProperties.Add(nameof(WorkItem.ScheduleId), item.ScheduleId);
            message.UserProperties.Add(nameof(WorkItem.Name), item.Name);
            return _sender.SendAsync(message);
        }

        public async Task<List<WorkItem>> ReceiveAsync(string worker, CancellationToken cancellationToken)
        {
            var messages = await _receiver.ReceiveAsync(_batchSize, TimeSpan.MaxValue);
            return messages.Select(message => new WorkItem
            {
                Id = new Guid(message.MessageId),
                Token = message.SystemProperties.LockToken,
                Payload = message.Body,
                CorrelationId = new Guid(message.CorrelationId),
                Attempts = message.SystemProperties.DeliveryCount,
                ScheduleId = (Guid?)message.UserProperties[nameof(WorkItem.ScheduleId)],
                Name = (string)message.UserProperties[nameof(WorkItem.Name)],
                RawMessage = message
            }).ToList();
        }

        public Task<DateTime> RenewAsync(WorkItem workItem, CancellationToken cancellationToken)
        {
            return _receiver.RenewLockAsync(workItem.Token);
        }

        public Task CompleteAsync(WorkItem workItem, CancellationToken cancellationToken)
        {
            return _receiver.CompleteAsync(workItem.Token);
        }

        public Task FaultAsync(WorkItem workItem, CancellationToken cancellationToken)
        {
            return _receiver.AbandonAsync(workItem.Token);
        }

        public void Dispose()
        {
        }
    }
}