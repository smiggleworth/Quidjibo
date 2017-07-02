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
        private readonly QueueClient _client;
        private readonly int _batchSize;

        public ServiceBusWorkProvider(QueueClient client, int batchSize)
        {
            _client = client;
            _batchSize = batchSize;
        }

        public  Task SendAsync(WorkItem item, int delay, CancellationToken cancellationToken)
        {
            var message = new Message(item.Payload)
            {
                MessageId = item.Id.ToString(),
                TimeToLive = item.ExpireOn - DateTime.UtcNow,
                CorrelationId = item.CorrelationId.ToString(),
                ScheduledEnqueueTimeUtc = DateTime.UtcNow.AddSeconds(delay)
            };
            return _client.SendAsync(message);
        }

        public async Task<List<WorkItem>> ReceiveAsync(string worker, CancellationToken cancellationToken)
        {
            await Task.Yield();
            var messages = new List<Message>();//await _client.ReceiveAsync(_batchSize);
            return messages.Select(message => new WorkItem
            {
                Id = new Guid(message.MessageId),
                Token = message.SystemProperties.LockToken,
                Payload = message.Body,
                CorrelationId = new Guid(message.CorrelationId),
                Attempts = message.SystemProperties.DeliveryCount
            }).ToList();
        }

        public Task<DateTime> RenewAsync(WorkItem workItem, CancellationToken cancellationToken)
        {
            //return _client.RenewAsync(workItem.Token);
            return Task.FromResult(DateTime.Now);
        }

        public Task CompleteAsync(WorkItem workItem, CancellationToken cancellationToken)
        {
            return _client.CompleteAsync(workItem.Token);
        }

        public Task FaultAsync(WorkItem workItem, CancellationToken cancellationToken)
        {
            return _client.AbandonAsync(workItem.Token);
        }
    }
}