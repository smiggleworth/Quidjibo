using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Commands;
using Quidjibo.Extensions;
using Quidjibo.Factories;
using Quidjibo.Models;
using Quidjibo.Providers;
using Quidjibo.Serializers;

namespace Quidjibo.Clients
{
    public class QuidjiboClient : IQuidjiboClient
    {
        public static IQuidjiboClient Instance = new QuidjiboDumbClient();

        private static readonly ConcurrentDictionary<string, IWorkProvider> WorkProviders = new ConcurrentDictionary<string, IWorkProvider>();
        private static readonly ConcurrentDictionary<string, IScheduleProvider> ScheduleProviders = new ConcurrentDictionary<string, IScheduleProvider>();
        private readonly ICronProvider _cronProvider;
        private readonly IPayloadSerializer _payloadSerializer;
        private readonly IScheduleProviderFactory _scheduleProviderFactory;

        private readonly IWorkProviderFactory _workProviderFactory;

        public QuidjiboClient(
            IWorkProviderFactory workProviderFactory,
            IScheduleProviderFactory scheduleProviderFactory,
            IPayloadSerializer payloadSerializer,
            ICronProvider cronProvider)
        {
            _workProviderFactory = workProviderFactory;
            _scheduleProviderFactory = scheduleProviderFactory;
            _payloadSerializer = payloadSerializer;
            _cronProvider = cronProvider;
        }

        public async Task PublishAsync(IQuidjiboCommand command,
            CancellationToken cancellationToken = new CancellationToken())
        {
            await PublishAsync(command, 0, cancellationToken);
        }

        public async Task PublishAsync(IQuidjiboCommand command, int delay,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var queueName = command.GetQueueName();
            await PublishAsync(command, queueName, delay, cancellationToken);
        }

        public async Task PublishAsync(IQuidjiboCommand command, string queueName,
            CancellationToken cancellationToken = new CancellationToken())
        {
            await PublishAsync(command, queueName, 0, cancellationToken);
        }

        public async Task PublishAsync(IQuidjiboCommand command, string queueName, int delay,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var item = new WorkItem
            {
                Id = Guid.NewGuid(),
                CorrelationId = Guid.NewGuid(),
                Name = command.GetName(),
                Attempts = 0,
                Payload = _payloadSerializer.Serialize(command),
                Queue = queueName
            };
            var provider = await GetOrCreateWorkProvider(queueName, cancellationToken);
            await provider.SendAsync(item, 0, cancellationToken);
        }

        public async Task ScheduleAsync(string name, IQuidjiboCommand command, Cron cron, CancellationToken cancellationToken = default(CancellationToken))
        {
            var queueName = command.GetQueueName();
            await ScheduleAsync(name, queueName, command, cron, cancellationToken);
        }

        public async Task ScheduleAsync(string name, string queue, IQuidjiboCommand command, Cron cron, CancellationToken cancellationToken = new CancellationToken())
        {
            var now = DateTime.UtcNow;
            var item = new ScheduleItem
            {
                CreatedOn = now,
                CronExpression = cron.Expression,
                EnqueueOn = _cronProvider.GetNextSchedule(cron.Expression),
                Id = Guid.NewGuid(),
                Name = name,
                Payload = _payloadSerializer.Serialize(command),
                Queue = queue,
                VisibleOn = now
            };
            var provider = await GetOrCreateScheduleProvider(queue, cancellationToken);
            var exists = await provider.ExistsAsync(name, cancellationToken);
            if (exists)
            {
                await provider.DeleteByNameAsync(name, cancellationToken);
            }
            await provider.CreateAsync(item, cancellationToken);
        }

        public void Dispose()
        {
            Instance = null;
            WorkProviders.Clear();
            ScheduleProviders.Clear();
        }

        private async Task<IWorkProvider> GetOrCreateWorkProvider(string queueName, CancellationToken cancellationToken)
        {
            IWorkProvider provider;
            if (!WorkProviders.TryGetValue(queueName, out provider))
            {
                provider = await _workProviderFactory.CreateAsync(queueName, cancellationToken);
                WorkProviders.TryAdd(queueName, provider);
            }
            return provider;
        }

        private async Task<IScheduleProvider> GetOrCreateScheduleProvider(string queueName,
            CancellationToken cancellationToken)
        {
            IScheduleProvider provider;
            if (!ScheduleProviders.TryGetValue(queueName, out provider))
            {
                provider = await _scheduleProviderFactory.CreateAsync(queueName, cancellationToken);
                ScheduleProviders.TryAdd(queueName, provider);
            }
            return provider;
        }
    }
}