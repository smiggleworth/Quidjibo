using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Quidjibo.Commands;
using Quidjibo.Extensions;
using Quidjibo.Factories;
using Quidjibo.Models;
using Quidjibo.Protectors;
using Quidjibo.Providers;
using Quidjibo.Serializers;

namespace Quidjibo.Clients
{
    public class SchedulerClient : ISchedulerClient
    {
        private static readonly ConcurrentDictionary<string, IScheduleProvider> Providers =
            new ConcurrentDictionary<string, IScheduleProvider>();

        private readonly ICronProvider _cronProvider;

        private readonly IPayloadSerializer _payloadSerializer;
        private readonly IScheduleProviderFactory _scheduleProviderFactory;

        public SchedulerClient(IScheduleProviderFactory scheduleProviderFactory) : this(
            scheduleProviderFactory,
            new PayloadSerializer(new PayloadProtector()),
            new CronProvider()) { }

        public SchedulerClient(
            IScheduleProviderFactory scheduleProviderFactory,
            IPayloadSerializer payloadSerializer,
            ICronProvider cronProvider)
        {
            _scheduleProviderFactory = scheduleProviderFactory;
            _payloadSerializer = payloadSerializer;
            _cronProvider = cronProvider;
        }

        public async Task ScheduleAsync(string name, IWorkCommand command, Cron cron, CancellationToken cancellationToken = default(CancellationToken))
        {
            var queueName = command.GetQueueName();
            await ScheduleAsync(name, queueName, command, cron, cancellationToken);
        }

        public async Task ScheduleAsync(string name, string queue, IWorkCommand command, Cron cron, CancellationToken cancellationToken = new CancellationToken())
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

        private async Task<IScheduleProvider> GetOrCreateScheduleProvider(string queueName,
            CancellationToken cancellationToken)
        {
            IScheduleProvider provider;
            if (!Providers.TryGetValue(queueName, out provider))
            {
                provider = await _scheduleProviderFactory.CreateAsync(queueName, cancellationToken);
                Providers.TryAdd(queueName, provider);
            }
            return provider;
        }
    }
}